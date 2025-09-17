using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ConnectFlow.Infrastructure.Data.Interceptors;

public class TenantFilterInterceptor : SaveChangesInterceptor
{
    private readonly IServiceProvider _serviceProvider;
    private IContextManager? _contextManager;

    public TenantFilterInterceptor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private IContextManager ContextManager => _contextManager ??= _serviceProvider.GetRequiredService<IContextManager>();

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
        if (ContextManager.IsSuperAdmin())
        {
            return;
        }
        else
        {
            // Get the tenant ID from the current tenant service, This will work in both HTTP and non-HTTP contexts, because TenantInfo uses AsyncLocal storage
            var currentTenantId = ContextManager.GetCurrentTenantId();

            if (!currentTenantId.HasValue) return;

            // Set TenantId for all entities implementing ITenantableEntity
            foreach (var entry in context.ChangeTracker.Entries<ITenantableEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.TenantId = currentTenantId.Value;
                }
            }
        }
    }
}