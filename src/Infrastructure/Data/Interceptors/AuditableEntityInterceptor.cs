using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ConnectFlow.Infrastructure.Data.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly IServiceProvider _serviceProvider;
    private IContextManager? _contextManager;

    private readonly TimeProvider _dateTime;

    public AuditableEntityInterceptor(IServiceProvider serviceProvider, TimeProvider dateTime)
    {
        _serviceProvider = serviceProvider;
        _dateTime = dateTime;
    }

    private IContextManager ContextManager => _contextManager ??= _serviceProvider.GetRequiredService<IContextManager>();

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<BaseAuditableEntity>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                var utcNow = _dateTime.GetUtcNow();
                if (entry.State == EntityState.Added && !entry.Entity.CreatedBy.HasValue)
                {
                    entry.Entity.CreatedBy = ContextManager.GetCurrentApplicationUserId();
                    entry.Entity.Created = utcNow;
                }

                entry.Entity.LastModifiedBy = ContextManager.GetCurrentApplicationUserId();
                entry.Entity.LastModified = utcNow;
            }
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r => r.TargetEntry != null && r.TargetEntry.Metadata.IsOwned() && (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}