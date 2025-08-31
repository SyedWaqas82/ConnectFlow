using ConnectFlow.Application.Common.Exceptions;

namespace ConnectFlow.Application.Subscriptions.Queries.GetCheckoutSession;

public class GetCheckoutSessionQueryHandler : IRequestHandler<GetCheckoutSessionQuery, CheckoutSessionStatusDto>
{
    private readonly IPaymentService _paymentService;
    private readonly IContextManager _contextManager;
    private readonly IApplicationDbContext _context;

    public GetCheckoutSessionQueryHandler(IPaymentService paymentService, IContextManager contextManager, IApplicationDbContext context)
    {
        _paymentService = paymentService;
        _contextManager = contextManager;
        _context = context;
    }

    public async Task<CheckoutSessionStatusDto> Handle(GetCheckoutSessionQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _contextManager.GetCurrentTenantId();
        Guard.Against.Null(tenantId, nameof(tenantId), "Tenant ID is required.", () => new TenantNotFoundException("Tenant ID is required."));

        // Verify that this checkout session belongs to the current tenant's customer
        var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId.Value, cancellationToken);
        Guard.Against.Null(tenant, nameof(tenant), $"Tenant not found {tenantId.Value}", () => new TenantNotFoundException($"Tenant not found {tenantId.Value}"));

        // Get the checkout session from Stripe
        var checkoutSession = await _paymentService.GetCheckoutSessionAsync(request.SessionId, cancellationToken);

        // Verify the customer ID matches the tenant's Stripe customer ID
        if (!string.IsNullOrEmpty(tenant.PaymentProviderCustomerId) && checkoutSession.CustomerId != tenant.PaymentProviderCustomerId)
        {
            throw new ForbiddenAccessException($"Checkout session {request.SessionId} does not belong to tenant {tenantId.Value}");
        }

        // Map the checkout session to the DTO with status analysis
        var result = new CheckoutSessionStatusDto
        {
            SessionId = checkoutSession.Id,
            Status = checkoutSession.Status,
            CustomerId = checkoutSession.CustomerId,
            Url = checkoutSession.Url,
            IsCompleted = checkoutSession.Status.Equals("complete", StringComparison.OrdinalIgnoreCase),
            IsExpired = checkoutSession.Status.Equals("expired", StringComparison.OrdinalIgnoreCase),
            IsOpen = checkoutSession.Status.Equals("open", StringComparison.OrdinalIgnoreCase),
            Metadata = checkoutSession.Metadata
        };

        return result;
    }
}