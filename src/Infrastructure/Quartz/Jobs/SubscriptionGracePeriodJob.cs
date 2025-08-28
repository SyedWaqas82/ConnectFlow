using Quartz;
using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Infrastructure.Quartz.Jobs;

/// <summary>
/// Job that handles subscription grace period expiration and auto-downgrade logic
/// </summary>
[DisallowConcurrentExecution]
public class SubscriptionGracePeriodJob : BaseJob
{
    private readonly IContextManager _contextManager;
    private readonly IApplicationDbContext _context;
    private readonly SubscriptionSettings _subscriptionSettings;
    private readonly IPaymentService _paymentService;

    public SubscriptionGracePeriodJob(ILogger<SubscriptionGracePeriodJob> logger, IContextManager contextManager, IApplicationDbContext context, IOptions<SubscriptionSettings> subscriptionSettings, IPaymentService paymentService) : base(logger, contextManager)
    {
        _contextManager = contextManager;
        _context = context;
        _subscriptionSettings = subscriptionSettings.Value;
        _paymentService = paymentService;
    }

    protected override async Task ExecuteJobAsync(IJobExecutionContext context)
    {
        // Context is automatically initialized by BaseJob
        Logger.LogInformation("Starting subscription grace period check");

        await ProcessExpiredGracePeriodsAsync(context.CancellationToken);

        Logger.LogInformation("Completed subscription grace period check");
    }

    private async Task ProcessExpiredGracePeriodsAsync(CancellationToken cancellationToken)
    {
        // Find all subscriptions in grace period that have expired
        var expiredGracePeriodSubscriptions = await _context.Subscriptions
            .Include(s => s.Plan)
            .Include(s => s.Tenant)
            .Where(s => s.IsInGracePeriod &&
                       s.GracePeriodEndsAt.HasValue &&
                       s.GracePeriodEndsAt.Value <= DateTimeOffset.UtcNow &&
                       s.Status != SubscriptionStatus.Canceled) // Don't process already canceled subscriptions
            .ToListAsync(cancellationToken);

        // Find subscriptions that have reached max retries but haven't been processed yet
        // Only process these if Stripe's retry period has also ended to avoid conflicts
        var maxRetriesSubscriptions = await _context.Subscriptions
            .Include(s => s.Plan)
            .Include(s => s.Tenant)
            .Where(s => s.HasReachedMaxRetries &&
                       s.Status == SubscriptionStatus.PastDue &&
                       s.FirstPaymentFailureAt.HasValue &&
                       s.FirstPaymentFailureAt.Value.AddDays(_subscriptionSettings.StripeRetryPeriodDays) <= DateTimeOffset.UtcNow &&
                       !s.IsInGracePeriod) // Ensure grace period isn't active
            .ToListAsync(cancellationToken);

        var allSubscriptionsToProcess = expiredGracePeriodSubscriptions
            .Concat(maxRetriesSubscriptions)
            .GroupBy(s => s.Id)
            .Select(g => g.First()) // Remove duplicates
            .ToList();

        Logger.LogInformation("Found {ExpiredCount} subscriptions with expired grace periods and {MaxRetriesCount} subscriptions past Stripe retry period",
            expiredGracePeriodSubscriptions.Count, maxRetriesSubscriptions.Count);

        foreach (var subscription in allSubscriptionsToProcess)
        {
            try
            {
                await ProcessExpiredGracePeriodSubscriptionAsync(subscription, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error processing expired grace period for subscription {SubscriptionId}", subscription.Id);
                // Continue with other subscriptions
            }
        }

        if (allSubscriptionsToProcess.Count > 0)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task ProcessExpiredGracePeriodSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Processing expired grace period for subscription {SubscriptionId} of tenant {TenantId} - RetryCount: {RetryCount}, HasReachedMaxRetries: {HasReachedMaxRetries}", subscription.Id, subscription.TenantId, subscription.PaymentRetryCount, subscription.HasReachedMaxRetries);

        await _contextManager.InitializeContextWithDefaultAdminAsync(subscription.TenantId);

        var currentPlan = subscription.Plan;

        // End the grace period
        subscription.IsInGracePeriod = false;
        subscription.GracePeriodEndsAt = null;

        // First, try to cancel the subscription in Stripe to maintain consistency
        try
        {
            if (!string.IsNullOrEmpty(subscription.PaymentProviderSubscriptionId) && !subscription.PaymentProviderSubscriptionId.StartsWith("free_"))
            {
                await _paymentService.CancelSubscriptionAsync(subscription.PaymentProviderSubscriptionId, cancelImmediately: true, cancellationToken);
                Logger.LogInformation("Successfully canceled subscription {SubscriptionId} in Stripe", subscription.Id);
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to cancel subscription {SubscriptionId} in Stripe, proceeding with local cancellation", subscription.Id);
            // Continue with local cancellation even if Stripe fails
        }

        // Determine if we should auto-downgrade based on retry status
        var shouldAutoDowngrade = _subscriptionSettings.AutoDowngradeAfterGracePeriod || (subscription.HasReachedMaxRetries && _subscriptionSettings.AutoDowngradeAfterMaxRetries);

        if (shouldAutoDowngrade)
        {
            // Find the free plan
            var freePlan = await _context.Plans.FirstOrDefaultAsync(p => p.Name.ToLower() == _subscriptionSettings.DefaultDowngradePlanName.ToLower() && p.IsActive, cancellationToken);

            if (freePlan != null)
            {
                // Cancel current subscription
                subscription.Status = SubscriptionStatus.Canceled;
                subscription.CanceledAt = DateTimeOffset.UtcNow;
                subscription.CancellationRequestedAt = subscription.CancellationRequestedAt ?? DateTimeOffset.UtcNow;

                // Reset retry tracking since we're moving to free plan
                subscription.PaymentRetryCount = 0;
                subscription.FirstPaymentFailureAt = null;
                subscription.NextRetryAt = null;
                subscription.HasReachedMaxRetries = false;

                // Add cancellation event for the current subscription
                subscription.AddDomainEvent(new SubscriptionStatusEvent(subscription.TenantId, default, subscription.Id, SubscriptionAction.Cancel, "Subscription canceled due to grace period expiration - auto-downgrading to free plan", sendEmailNotification: true, suspendLimitsImmediately: true));     // Grace period expiration cancellation suspends limits immediately

                // Create new free subscription
                var freeSubscription = new Subscription
                {
                    PaymentProviderSubscriptionId = $"free_{Guid.NewGuid()}",
                    Status = SubscriptionStatus.Active,
                    CurrentPeriodStart = DateTimeOffset.UtcNow,
                    CurrentPeriodEnd = DateTimeOffset.UtcNow.AddYears(100), // Free plan never expires
                    CancelAtPeriodEnd = false,
                    PlanId = freePlan.Id,
                    TenantId = subscription.TenantId
                };

                _context.Subscriptions.Add(freeSubscription);

                // Add event for new free subscription creation
                freeSubscription.AddDomainEvent(new SubscriptionStatusEvent(subscription.TenantId, default,
                    freeSubscription.Id,
                    SubscriptionAction.Create,
                    $"Auto-downgraded from {currentPlan.Name} to {freePlan.Name} after grace period expiration",
                    sendEmailNotification: true));

                Logger.LogInformation("Auto-downgraded subscription {SubscriptionId} from {FromPlan} to {ToPlan} for tenant {TenantId}",
                    subscription.Id, currentPlan.Name, freePlan.Name, subscription.TenantId);
            }
            else
            {
                Logger.LogWarning("Free plan '{PlanName}' not found for auto-downgrade of subscription {SubscriptionId}",
                    _subscriptionSettings.DefaultDowngradePlanName, subscription.Id);

                // Just cancel the subscription without downgrade
                subscription.Status = SubscriptionStatus.Canceled;
                subscription.CanceledAt = DateTimeOffset.UtcNow;
                subscription.CancellationRequestedAt = subscription.CancellationRequestedAt ?? DateTimeOffset.UtcNow;

                // Add cancellation event
                subscription.AddDomainEvent(new SubscriptionStatusEvent(subscription.TenantId, default, subscription.Id, SubscriptionAction.Cancel, "Subscription canceled due to grace period expiration - free plan not available", sendEmailNotification: true, suspendLimitsImmediately: true)); // Grace period expiration cancellation suspends limits immediately
            }
        }
        else
        {
            // Just cancel the subscription
            subscription.Status = SubscriptionStatus.Canceled;
            subscription.CanceledAt = DateTimeOffset.UtcNow;
            subscription.CancellationRequestedAt = subscription.CancellationRequestedAt ?? DateTimeOffset.UtcNow;

            // Add cancellation event
            subscription.AddDomainEvent(new SubscriptionStatusEvent(subscription.TenantId, default, subscription.Id, SubscriptionAction.Cancel, "Subscription canceled due to grace period expiration", sendEmailNotification: true, suspendLimitsImmediately: true)); // Grace period expiration cancellation suspends limits immediately
        }

        // Add grace period end event
        subscription.AddDomainEvent(new SubscriptionStatusEvent(subscription.TenantId, default, subscription.Id, SubscriptionAction.GracePeriodEnd, $"Grace period ended for {currentPlan.Name} plan", sendEmailNotification: true));
    }
}