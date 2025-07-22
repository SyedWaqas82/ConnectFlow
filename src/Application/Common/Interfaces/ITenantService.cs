using ConnectFlow.Domain.Entities;

namespace ConnectFlow.Application.Common.Interfaces;

public interface ITenantService
{
    string? GetCurrentTenantId();
    Task<Tenant?> GetCurrentTenantAsync();
    Task<Tenant?> FindTenantByIdentifierAsync(string identifier);
    Task EnsureValidTenantAsync();
}
