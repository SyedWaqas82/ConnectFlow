namespace ConnectFlow.Domain.Constants;

public class EmailTemplates
{
    public const string Welcome = nameof(Welcome);
    public const string PasswordReset = nameof(PasswordReset);

    // Payment templates
    public const string PaymentSuccess = nameof(PaymentSuccess);
    public const string PaymentFailed = nameof(PaymentFailed);
    public const string PaymentRetry = nameof(PaymentRetry);
    public const string PaymentRefundRequested = nameof(PaymentRefundRequested);
    public const string PaymentRefunded = nameof(PaymentRefunded);

    // Subscription templates
    public const string SubscriptionCreated = nameof(SubscriptionCreated);
    public const string SubscriptionSuspended = nameof(SubscriptionSuspended);
    public const string SubscriptionReactivated = nameof(SubscriptionReactivated);
    public const string SubscriptionCancelled = nameof(SubscriptionCancelled);
    public const string SubscriptionGracePeriodStart = nameof(SubscriptionGracePeriodStart);
    public const string SubscriptionGracePeriodEnd = nameof(SubscriptionGracePeriodEnd);
    public const string SubscriptionPlanChanged = nameof(SubscriptionPlanChanged);

    // Tenant User templates
    public const string TenantUserSuspended = nameof(TenantUserSuspended);
    public const string TenantUserRestored = nameof(TenantUserRestored);
}