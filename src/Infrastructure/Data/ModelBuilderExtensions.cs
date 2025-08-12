using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace ConnectFlow.Infrastructure.Data;

public static class ModelBuilderExtensions
{
    public static void ApplyTenantFilters(this ModelBuilder modelBuilder, IContextManager contextManager)
    {
        var tenantEntityTypes = modelBuilder.Model.GetEntityTypes().Where(e => typeof(ITenantEntity).IsAssignableFrom(e.ClrType)).ToList();

        foreach (var entityType in tenantEntityTypes)
        {
            var entityClrType = entityType.ClrType;
            var method = typeof(ModelBuilderExtensions).GetMethod(nameof(SetTenantFilter), BindingFlags.NonPublic | BindingFlags.Static)?.MakeGenericMethod(entityClrType);

            method?.Invoke(null, new object[] { modelBuilder, contextManager });
        }
    }

    public static void ApplySoftDeleteFilters(this ModelBuilder modelBuilder)
    {
        var softDeleteEntityTypes = modelBuilder.Model.GetEntityTypes().Where(e => typeof(ISoftDelete).IsAssignableFrom(e.ClrType)).ToList();

        foreach (var entityType in softDeleteEntityTypes)
        {
            var entityClrType = entityType.ClrType;
            var method = typeof(ModelBuilderExtensions).GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)?.MakeGenericMethod(entityClrType);

            method?.Invoke(null, new object[] { modelBuilder });
        }
    }

    private static void SetTenantFilter<T>(ModelBuilder modelBuilder, IContextManager contextManager) where T : class, ITenantEntity
    {
        var tenantId = contextManager.GetCurrentTenantId();
        modelBuilder.Entity<T>().HasQueryFilter(e => contextManager.IsSuperAdmin() || (tenantId.HasValue && e.TenantId == tenantId.Value));
    }

    private static void SetSoftDeleteFilter<T>(ModelBuilder modelBuilder) where T : class, ISoftDelete
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
    }
}