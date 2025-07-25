using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Application.Common.Services;
using ConnectFlow.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ConnectFlow.Infrastructure.Data.Interceptors;

public class TenantFilterInterceptor : SaveChangesInterceptor
{
    private readonly ITenantService _tenantService;

    public TenantFilterInterceptor(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateTenantId(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateTenantId(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateTenantId(DbContext? context)
    {
        if (context == null) return;

        // Don't add tenant filter for Super Admin users
        if (_tenantService.IsSuperAdmin())
        {
            return;
        }
        else
        {
            // Get the tenant ID from AsyncLocal storage
            var currentTenantId = TenantInfo.CurrentTenantId;
            if (!currentTenantId.HasValue) return;

            // Set TenantId for all entities implementing ITenantEntity
            foreach (var entry in context.ChangeTracker.Entries<ITenantEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.TenantId = currentTenantId.Value;
                }
            }
        }
    }
}