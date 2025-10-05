using ConnectFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public abstract class BaseAuditableConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseAuditableEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();
        builder.HasIndex(t => t.PublicId).IsUnique();
        builder.Property(t => t.PublicId).HasDefaultValueSql("gen_random_uuid()").IsRequired(); // PostgreSQL
        //builder.Property(t => t.PublicId).HasDefaultValueSql("newid()").IsRequired(); // SQL Server
        //builder.Property(t => t.Created).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(t => t.Created).HasDefaultValueSql("now()"); // PostgreSQL default value
        builder.Property(t => t.LastModified).HasDefaultValueSql("now()"); // PostgreSQL default value

        // Common navigation properties
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(t => t.CreatedBy).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(t => t.LastModifiedBy).OnDelete(DeleteBehavior.SetNull);

        // Generic tenant and soft delete relationships
        ConfigureTenantRelationship(builder);
        ConfigureSoftDelete(builder);
        ConfigureEntitySuspension(builder);
    }

    protected virtual void ConfigureTenantRelationship(EntityTypeBuilder<TEntity> builder)
    {
        if (typeof(ITenantableEntity).IsAssignableFrom(typeof(TEntity)))
        {
            // Basic tenant index for all tenant-related entities
            builder.HasIndex("TenantId").HasFilter(null).HasDatabaseName($"IX_{typeof(TEntity).Name}_TenantId");
            builder.Property("TenantId").IsRequired();

            // Add index on Created date which is useful for many reporting queries
            if (typeof(BaseAuditableEntity).IsAssignableFrom(typeof(TEntity)))
            {
                // Created date filtering within a tenant context is a common pattern
                builder.HasIndex(new[] { "TenantId", "Created" }).HasDatabaseName($"IX_{typeof(TEntity).Name}_TenantId_Created");
            }

            // Try to find a navigation property named "Tenant"
            var nav = typeof(TEntity).GetProperty("Tenant");
            if (nav != null)
            {
                // Configure the relationship only if the navigation exists
                builder.HasOne(typeof(Tenant), "Tenant").WithMany().HasForeignKey("TenantId").OnDelete(DeleteBehavior.Cascade);
            }
        }
    }

    protected virtual void ConfigureSoftDelete(EntityTypeBuilder<TEntity> builder)
    {
        if (typeof(ISoftDeleteableEntity).IsAssignableFrom(typeof(TEntity)))
        {
            builder.Property<bool>("IsDeleted").HasDefaultValue(false);

            // Add composite index for filtering non-deleted entities within a tenant
            if (typeof(ITenantableEntity).IsAssignableFrom(typeof(TEntity)))
            {
                builder.HasIndex(new[] { "TenantId", "IsDeleted" }).HasDatabaseName($"IX_{typeof(TEntity).Name}_TenantId_IsDeleted");
            }
            // Add standalone index for IsDeleted for non-tenant entities
            else
            {
                builder.HasIndex("IsDeleted").HasDatabaseName($"IX_{typeof(TEntity).Name}_IsDeleted");
            }

            builder.HasOne<ApplicationUser>().WithMany().HasForeignKey("DeletedBy").OnDelete(DeleteBehavior.SetNull);
        }
    }

    protected virtual void ConfigureEntitySuspension(EntityTypeBuilder<TEntity> builder)
    {
        if (typeof(ISuspendibleEntity).IsAssignableFrom(typeof(TEntity)))
        {
            builder.Property<EntityStatus>("EntityStatus").IsRequired().HasConversion<string>();

            // Add composite index for filtering by status within a tenant
            if (typeof(ITenantableEntity).IsAssignableFrom(typeof(TEntity)))
            {
                builder.HasIndex(new[] { "TenantId", "EntityStatus" }).HasDatabaseName($"IX_{typeof(TEntity).Name}_TenantId_EntityStatus");
            }
            // Add standalone index for EntityStatus for non-tenant entities
            else
            {
                builder.HasIndex("EntityStatus").HasDatabaseName($"IX_{typeof(TEntity).Name}_EntityStatus");
            }
        }
    }
}