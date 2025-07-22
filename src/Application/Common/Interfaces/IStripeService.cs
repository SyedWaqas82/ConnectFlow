using ConnectFlow.Domain.Entities;
using ConnectFlow.Domain.Enums;

namespace ConnectFlow.Application.Common.Interfaces;

public interface IStripeService
{
    Task<Subscription> CreateSubscriptionAsync(string tenantId, string email, string paymentMethodId, SubscriptionPlan plan);
    Task CancelSubscriptionAsync(string subscriptionId, string? reason = null);
}
