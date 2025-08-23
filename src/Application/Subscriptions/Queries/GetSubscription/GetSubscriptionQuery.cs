using ConnectFlow.Application.Common.Security;
using ConnectFlow.Domain.Constants;

namespace ConnectFlow.Application.Subscriptions.Queries.GetSubscription;

[AuthorizeTenantSubscription(true, Roles.TenantAdmin)]
public record GetSubscriptionQuery : IRequest<SubscriptionDto>;

public class GetSubscriptionQueryHandler : IRequestHandler<GetSubscriptionQuery, SubscriptionDto>
{
    private readonly ISubscriptionManagementService _subscriptionManagementService;
    private readonly IContextManager _contextManager;
    private readonly IMapper _mapper;

    public GetSubscriptionQueryHandler(ISubscriptionManagementService subscriptionManagementService, IContextManager contextManager, IMapper mapper)
    {
        _subscriptionManagementService = subscriptionManagementService;
        _contextManager = contextManager;
        _mapper = mapper;
    }

    public async Task<SubscriptionDto> Handle(GetSubscriptionQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _contextManager.GetCurrentTenantId();

        Guard.Against.Null(tenantId, nameof(tenantId));

        var subscription = await _subscriptionManagementService.GetActiveSubscriptionAsync(tenantId.Value, cancellationToken);
        var usage = await _subscriptionManagementService.GetUsageStatisticsAsync(tenantId.Value, cancellationToken);

        Guard.Against.Null(subscription, message: "Subscription not found");

        // Map subscription to DTO using AutoMapper
        var subscriptionDto = _mapper.Map<SubscriptionDto>(subscription);

        // Map usage statistics separately and assign to the DTO
        var usageDto = _mapper.Map<UsageDto>(usage);

        return subscriptionDto with { Usage = usageDto };
    }
}