using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ConnectFlow.Infrastructure.Data;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Applies the tenant global query filter to all entities that implement ITenantEntity
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    /// <param name="tenantService">The tenant resolver service</param>
    public static void ApplyTenantFilters(this ModelBuilder modelBuilder, ITenantService tenantService)
    {
        // Get all entity types that implement ITenantEntity
        var tenantEntityTypes = modelBuilder.Model.GetEntityTypes().Where(e => typeof(ITenantEntity).IsAssignableFrom(e.ClrType)).ToList();

        foreach (var entityType in tenantEntityTypes)
        {
            var entityClrType = entityType.ClrType;

            // Create a generic method to apply the filter
            var method = typeof(ModelBuilderExtensions).GetMethod(nameof(SetTenantFilter), BindingFlags.NonPublic | BindingFlags.Static)?.MakeGenericMethod(entityClrType);

            method?.Invoke(null, new object[] { modelBuilder, tenantService });
        }
    }

    /// <summary>
    /// Applies the soft delete global query filter to all entities that implement ISoftDelete
    /// </summary>
    /// <param name="modelBuilder">The model builder</param>
    public static void ApplySoftDeleteFilters(this ModelBuilder modelBuilder)
    {
        // Get all entity types that implement ISoftDelete
        var softDeleteEntityTypes = modelBuilder.Model.GetEntityTypes().Where(e => typeof(ISoftDelete).IsAssignableFrom(e.ClrType)).ToList();

        foreach (var entityType in softDeleteEntityTypes)
        {
            var entityClrType = entityType.ClrType;

            // Create a generic method to apply the filter
            var method = typeof(ModelBuilderExtensions).GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)?.MakeGenericMethod(entityClrType);

            method?.Invoke(null, new object[] { modelBuilder });
        }
    }

    /// <summary>
    /// Sets the tenant filter for a specific entity type
    /// </summary>
    private static void SetTenantFilter<T>(ModelBuilder modelBuilder, ITenantService tenantService) where T : class, ITenantEntity
    {
        // If user is SuperAdmin, return all entities (no tenant filtering) otherwise, filter by the current tenant ID
        modelBuilder.Entity<T>().HasQueryFilter(e => !tenantService.IsSuperAdmin() && (tenantService.GetCurrentTenantIdAsync().Result == e.TenantId));
    }

    /// <summary>
    /// Sets the soft delete filter for a specific entity type
    /// </summary>
    private static void SetSoftDeleteFilter<T>(ModelBuilder modelBuilder) where T : class, ISoftDelete
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
    }
}