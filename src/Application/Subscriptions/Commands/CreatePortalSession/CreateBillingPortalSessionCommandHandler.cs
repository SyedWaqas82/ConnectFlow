using ConnectFlow.Application.Common.Exceptions;
using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Application.Subscriptions.Commands.CreatePortalSession;

public class CreateBillingPortalSessionCommandHandler : IRequestHandler<CreateBillingPortalSessionCommand, Result<CreateBillingPortalSessionResult>>
{
    private readonly IContextManager _contextManager;
    private readonly IApplicationDbContext _context;
    private readonly IPaymentService _paymentService;

    public CreateBillingPortalSessionCommandHandler(IContextManager contextManager, IApplicationDbContext context, IPaymentService paymentService)
    {
        _contextManager = contextManager;
        _context = context;
        _paymentService = paymentService;
    }

    public async Task<Result<CreateBillingPortalSessionResult>> Handle(CreateBillingPortalSessionCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _contextManager.GetCurrentTenantId();
        Guard.Against.Null(tenantId, nameof(tenantId), "Tenant ID not found", () => new TenantNotFoundException("Tenant ID not found"));

        var tenant = await _context.Tenants.FindAsync(tenantId);
        Guard.Against.Null(tenant, nameof(tenant), "Tenant not found", () => new TenantNotFoundException("Tenant not found"));

        var result = await _paymentService.CreateBillingPortalSessionAsync(tenant.PaymentProviderCustomerId, request.ReturnUrl, cancellationToken);

        return Result<CreateBillingPortalSessionResult>.Success(new CreateBillingPortalSessionResult
        {
            Url = result.Url
        });
    }
}