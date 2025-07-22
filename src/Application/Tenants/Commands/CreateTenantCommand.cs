using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Domain.Entities;

namespace ConnectFlow.Application.Tenants.Commands;

public record CreateTenantCommand : IRequest<Tenant>
{
    public string Name { get; init; } = string.Empty;
    public string Subdomain { get; init; } = string.Empty;
    public string? LogoUrl { get; init; }
    public bool IsActive { get; init; }
}

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Tenant>
{
    private readonly IApplicationDbContext _context;

    public CreateTenantCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Tenant> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = new Tenant
        {
            Name = request.Name,
            Subdomain = request.Subdomain,
            LogoUrl = request.LogoUrl,
            IsActive = request.IsActive,
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync(cancellationToken);

        return tenant;
    }
}