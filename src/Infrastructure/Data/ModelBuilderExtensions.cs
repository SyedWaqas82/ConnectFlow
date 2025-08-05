using ConnectFlow.Domain.Common;
using ConnectFlow.Infrastructure.Common.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ConnectFlow.Infrastructure.Data;

public static class ModelBuilderExtensions
{
    public static void ApplyTenantFilters(this ModelBuilder modelBuilder)
    {
        var tenantEntityTypes = modelBuilder.Model.GetEntityTypes().Where(e => typeof(ITenantEntity).IsAssignableFrom(e.ClrType)).ToList();

        foreach (var entityType in tenantEntityTypes)
        {
            var entityClrType = entityType.ClrType;
            var method = typeof(ModelBuilderExtensions).GetMethod(nameof(SetTenantFilter), BindingFlags.NonPublic | BindingFlags.Static)?.MakeGenericMethod(entityClrType);

            method?.Invoke(null, new object[] { modelBuilder });
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

    private static void SetTenantFilter<T>(ModelBuilder modelBuilder) where T : class, ITenantEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => UserInfo.IsSuperAdmin || (TenantInfo.CurrentTenantId.HasValue && e.TenantId == TenantInfo.CurrentTenantId.Value));
    }

    private static void SetSoftDeleteFilter<T>(ModelBuilder modelBuilder) where T : class, ISoftDelete
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
    }
}