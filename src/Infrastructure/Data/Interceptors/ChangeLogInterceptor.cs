using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace ConnectFlow.Infrastructure.Data.Interceptors;

/// <summary>
/// EF Core interceptor that automatically creates change logs for entities implementing IChangeLogableEntity
/// </summary>
public class ChangeLogInterceptor : SaveChangesInterceptor
{
    private readonly IServiceProvider _serviceProvider;
    private IContextManager? _contextManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ChangeLogInterceptor(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor)
    {
        _serviceProvider = serviceProvider;
        _httpContextAccessor = httpContextAccessor;
    }

    private IContextManager ContextManager => _contextManager ??= _serviceProvider.GetRequiredService<IContextManager>();

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        CreateChangeLogs(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        CreateChangeLogs(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void CreateChangeLogs(DbContext? context)
    {
        if (context == null) return;

        var currentUser = ContextManager.GetCurrentApplicationUserId();
        var tenantId = ContextManager.GetCurrentTenantId() ?? 0;
        var httpContext = _httpContextAccessor.HttpContext;
        var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString();
        var userAgent = httpContext?.Request?.Headers["User-Agent"].ToString();
        var now = DateTimeOffset.UtcNow;

        var entries = context.ChangeTracker.Entries().Where(e => e.Entity is IChangeLogableEntity && (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)).ToList();

        foreach (var entry in entries)
        {
            var entity = (IChangeLogableEntity)entry.Entity;
            var loggableFields = entity.GetLoggableFields();
            var shouldTrackAllFields = loggableFields == null || !loggableFields.Any();

            switch (entry.State)
            {
                case EntityState.Added:
                    CreateEntityChangeLog(context, entity, ChangeType.Created, $"Created {entity.EntityType}", currentUser, tenantId, ipAddress, userAgent, now);
                    break;

                case EntityState.Deleted:
                    CreateEntityChangeLog(context, entity, ChangeType.Deleted, $"Deleted {entity.EntityType}", currentUser, tenantId, ipAddress, userAgent, now);
                    break;

                case EntityState.Modified:
                    foreach (var property in entry.Properties.Where(p => p.IsModified))
                    {
                        var propertyName = property.Metadata.Name;

                        // Skip properties that shouldn't be tracked
                        if (!shouldTrackAllFields && loggableFields != null && !loggableFields.Contains(propertyName))
                            continue;

                        // Skip audit fields to avoid circular updates
                        if (IsAuditField(propertyName))
                            continue;

                        var oldValue = entity.FormatValueForDisplay(propertyName, property.OriginalValue);
                        var newValue = entity.FormatValueForDisplay(propertyName, property.CurrentValue);
                        var displayName = entity.GetPropertyDisplayName(propertyName);

                        CreatePropertyChangeLog(context, entity, propertyName, displayName, oldValue, newValue, currentUser, tenantId, ipAddress, userAgent, now);
                    }
                    break;
            }
        }
    }

    private static void CreateEntityChangeLog(DbContext context, IChangeLogableEntity entity, ChangeType changeType, string description, int? currentUser, int tenantId, string? ipAddress, string? userAgent, DateTimeOffset now)
    {
        var changeLog = new ChangeLog
        {
            EntityId = entity.Id,
            EntityType = entity.EntityType,
            ChangeType = changeType,
            Description = description,
            TenantId = tenantId,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Created = now,
            CreatedBy = currentUser,
            LastModified = now,
            LastModifiedBy = currentUser
        };

        context.Set<ChangeLog>().Add(changeLog);
    }

    private static void CreatePropertyChangeLog(DbContext context, IChangeLogableEntity entity, string propertyName, string displayName, string? oldValue, string? newValue, int? currentUser, int tenantId, string? ipAddress, string? userAgent, DateTimeOffset now)
    {
        var changeLog = new ChangeLog
        {
            EntityId = entity.Id,
            EntityType = entity.EntityType,
            ChangeType = ChangeType.Updated,
            PropertyName = propertyName,
            PropertyDisplayName = displayName,
            OldValue = oldValue,
            NewValue = newValue,
            Description = $"Changed {displayName} from '{oldValue}' to '{newValue}'",
            TenantId = tenantId,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Created = now,
            CreatedBy = currentUser,
            LastModified = now,
            LastModifiedBy = currentUser
        };

        context.Set<ChangeLog>().Add(changeLog);
    }

    private static bool IsAuditField(string propertyName)
    {
        return propertyName is "Created" or "CreatedBy" or "LastModified" or "LastModifiedBy";
    }
}