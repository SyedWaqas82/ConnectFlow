namespace ConnectFlow.Application.Subscriptions.Queries.GetAvailablePlans;

public record GetAvailablePlansQuery : IRequest<List<PlanDto>>;

public class GetAvailablePlansQueryHandler : IRequestHandler<GetAvailablePlansQuery, List<PlanDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IContextManager _contextManager;
    private readonly ISubscriptionManagementService _subscriptionManagementService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAvailablePlansQueryHandler> _logger;

    public GetAvailablePlansQueryHandler(IApplicationDbContext context, IContextManager contextManager, ISubscriptionManagementService subscriptionManagementService, IMapper mapper, ILogger<GetAvailablePlansQueryHandler> logger)
    {
        _context = context;
        _contextManager = contextManager;
        _subscriptionManagementService = subscriptionManagementService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<PlanDto>> Handle(GetAvailablePlansQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _contextManager.GetCurrentTenantId();
        int? currentPlanId = null;

        if (tenantId.HasValue)
        {
            var currentSubscription = await _subscriptionManagementService.GetActiveSubscriptionAsync(tenantId.Value, cancellationToken);
            currentPlanId = currentSubscription?.PlanId;
        }

        var plans = await _context.Plans.AsNoTracking().Where(p => p.IsActive).OrderBy(p => p.Price).ProjectTo<PlanDto>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);

        if (currentPlanId.HasValue)
        {
            foreach (var plan in plans)
            {
                plan.IsCurrentPlan = plan.Id == currentPlanId.Value;
            }
        }

        return plans;
    }
}