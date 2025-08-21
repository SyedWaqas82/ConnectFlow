using ConnectFlow.Domain.Constants;
using ConnectFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.HasIndex(ar => ar.Name).IsUnique();
        builder.Property(ar => ar.Description).HasMaxLength(256);
        builder.Property(ar => ar.IsSystemRole).HasDefaultValue(false);
        //builder.Property(ar => ar.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP"); // Sql Server default value
        builder.Property(ar => ar.CreatedDate).HasDefaultValueSql("now()"); // PostgreSQL default value

        SeedRoles(builder);
    }

    private static void SeedRoles(EntityTypeBuilder<ApplicationRole> builder)
    {
        var roles = new[]
        {
                new ApplicationRole
                {
                    Id = 1,
                    Name = Roles.SuperAdmin,
                    NormalizedName = Roles.SuperAdmin.ToUpperInvariant(),
                    Description = "Full system access across all tenants",
                    IsSystemRole = true,
                    CreatedDate = new DateTime(2025, 07, 25, 0, 0, 0, DateTimeKind.Utc)
                },
                new ApplicationRole
                {
                    Id = 2,
                    Name = Roles.TenantAdmin,
                    NormalizedName = Roles.TenantAdmin.ToUpperInvariant(),
                    Description = "Full access within assigned tenant",
                    IsSystemRole = true,
                    CreatedDate = new DateTime(2025, 07, 25, 0, 0, 0, DateTimeKind.Utc)
                },
                new ApplicationRole
                {
                    Id = 3,
                    Name = Roles.NonTenantAdmin,
                    NormalizedName = Roles.NonTenantAdmin.ToUpperInvariant(),
                    Description = "Regular user access within assigned tenant",
                    IsSystemRole = true,
                    CreatedDate = new DateTime(2025, 07, 25, 0, 0, 0, DateTimeKind.Utc)
                }
            };

        builder.HasData(roles);
    }
}