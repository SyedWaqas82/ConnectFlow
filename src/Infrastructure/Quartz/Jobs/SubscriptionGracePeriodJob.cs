using Quartz;

namespace ConnectFlow.Infrastructure.Quartz.Jobs;

/// <summary>
/// Job that handles subscription grace period expiration and auto-downgrade logic
/// </summary>
[DisallowConcurrentExecution]
public class SubscriptionGracePeriodJob : BaseJob
{
    private readonly IContextManager _contextManager;
    private readonly IApplicationDbContext _context;
    private readonly IPaymentService _paymentService;

    public SubscriptionGracePeriodJob(ILogger<SubscriptionGracePeriodJob> logger, IContextManager contextManager, IApplicationDbContext context, IPaymentService paymentService) : base(logger, contextManager)
    {
        _contextManager = contextManager;
        _context = context;
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
        var currentTime = DateTimeOffset.UtcNow;

        // Unified query to find all subscriptions that need processing:
        // 1. Subscriptions with expired grace periods
        // 2. Subscriptions that have reached max retries
        var subscriptionsToProcess = await _context.Subscriptions
            .Include(s => s.Plan)
            .Include(s => s.Tenant)
            .Where(s => s.Status != SubscriptionStatus.Canceled && // Don't process already canceled subscriptions
                    (

                        (s.IsInGracePeriod && s.GracePeriodEndsAt.HasValue && s.GracePeriodEndsAt.Value <= currentTime) || // Condition 1: Grace period has expired
                        (s.HasReachedMaxRetries && (s.Status == SubscriptionStatus.PastDue || s.Status == SubscriptionStatus.Unpaid)) // Condition 2: Max retries reached (process immediately)
                    ))
            .ToListAsync(cancellationToken);

        Logger.LogInformation("Found {Count} subscriptions requiring processing: grace period expired or max retries reached", subscriptionsToProcess.Count);

        foreach (var subscription in subscriptionsToProcess)
        {
            try
            {
                var reason = subscription.IsInGracePeriod && subscription.GracePeriodEndsAt <= currentTime && !subscription.HasReachedMaxRetries ? "expired grace period" : "max retries reached";

                Logger.LogInformation("Processing subscription {SubscriptionId} for reason: {Reason}", subscription.Id, reason);

                await ProcessExpiredGracePeriodSubscriptionAsync(subscription, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error processing subscription {SubscriptionId}", subscription.Id);
                // Continue with other subscriptions
            }
        }
    }

    private async Task ProcessExpiredGracePeriodSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Processing expired grace period for subscription {SubscriptionId} of tenant {TenantId} - RetryCount: {RetryCount}, HasReachedMaxRetries: {HasReachedMaxRetries}", subscription.Id, subscription.TenantId, subscription.PaymentRetryCount, subscription.HasReachedMaxRetries);

        await _contextManager.InitializeContextWithDefaultAdminAsync(subscription.TenantId);

        // First, try to cancel the subscription in Stripe to maintain consistency
        try
        {
            if (!string.IsNullOrEmpty(subscription.PaymentProviderSubscriptionId) && subscription.Plan.Type != PlanType.Free)
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

        // End the grace period
        subscription.IsInGracePeriod = false;
        subscription.GracePeriodEndsAt = null;
        subscription.CanceledAt = DateTimeOffset.UtcNow;

        // Add cancellation event for the current subscription to immediately suspend limits
        subscription.AddDomainEvent(new SubscriptionStatusEvent(subscription.TenantId, default, subscription, SubscriptionAction.Cancel, "Subscription canceled due to grace period expiration - auto-downgrading to free plan", sendEmailNotification: true, isImmediate: true));

        await _context.SaveChangesAsync(cancellationToken); // Save cancellation event before creating new subscription

        Logger.LogInformation("Subscription {SubscriptionId} canceled - auto-downgraded from plan {FromPlan} to free plan for tenant {TenantId}", subscription.Id, subscription.Plan.Name, subscription.TenantId);
    }
}