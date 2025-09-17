using System.Reflection;


namespace ConnectFlow.Infrastructure.Data;

public static class ModelBuilderExtensions
{
    public static void ApplyTenantFilters(this ModelBuilder modelBuilder, IContextManager contextManager)
    {
        var tenantEntityTypes = modelBuilder.Model.GetEntityTypes().Where(e => typeof(ITenantableEntity).IsAssignableFrom(e.ClrType)).ToList();

        foreach (var entityType in tenantEntityTypes)
        {
            var entityClrType = entityType.ClrType;
            var method = typeof(ModelBuilderExtensions).GetMethod(nameof(SetTenantFilter), BindingFlags.NonPublic | BindingFlags.Static)?.MakeGenericMethod(entityClrType);

            method?.Invoke(null, new object[] { modelBuilder, contextManager });
        }
    }

    public static void ApplySoftDeleteFilters(this ModelBuilder modelBuilder)
    {
        var softDeleteEntityTypes = modelBuilder.Model.GetEntityTypes().Where(e => typeof(ISoftDeleteableEntity).IsAssignableFrom(e.ClrType)).ToList();

        foreach (var entityType in softDeleteEntityTypes)
        {
            var entityClrType = entityType.ClrType;
            var method = typeof(ModelBuilderExtensions).GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)?.MakeGenericMethod(entityClrType);

            method?.Invoke(null, new object[] { modelBuilder });
        }
    }

    public static void ApplySuspendibleFilters(this ModelBuilder modelBuilder)
    {
        var suspendibleEntityTypes = modelBuilder.Model.GetEntityTypes().Where(e => typeof(ISuspendibleEntity).IsAssignableFrom(e.ClrType)).ToList();

        foreach (var entityType in suspendibleEntityTypes)
        {
            var entityClrType = entityType.ClrType;
            var method = typeof(ModelBuilderExtensions).GetMethod(nameof(SetSuspendibleFilter), BindingFlags.NonPublic | BindingFlags.Static)?.MakeGenericMethod(entityClrType);

            method?.Invoke(null, new object[] { modelBuilder });
        }
    }

    private static void SetTenantFilter<T>(ModelBuilder modelBuilder, IContextManager contextManager) where T : class, ITenantableEntity
    {
        var tenantId = contextManager.GetCurrentTenantId();
        modelBuilder.Entity<T>().HasQueryFilter(e => contextManager.IsSuperAdmin() || (tenantId.HasValue && e.TenantId == tenantId.Value));
    }

    private static void SetSoftDeleteFilter<T>(ModelBuilder modelBuilder) where T : class, ISoftDeleteableEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
    }

    private static void SetSuspendibleFilter<T>(ModelBuilder modelBuilder) where T : class, ISuspendibleEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => e.EntityStatus == EntityStatus.Active);
    }
}