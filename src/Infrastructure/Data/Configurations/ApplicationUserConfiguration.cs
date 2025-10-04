using ConnectFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasIndex(au => au.PublicId).IsUnique();
        //builder.Property(au => au.PublicId).HasDefaultValueSql("newid()").IsRequired(); // SQL Server
        builder.Property(au => au.PublicId).HasDefaultValueSql("gen_random_uuid()").IsRequired(); // PostgreSQL
        builder.Property(au => au.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(au => au.LastName).IsRequired().HasMaxLength(50);
        builder.Property(au => au.TimeZone).IsRequired().HasMaxLength(50);
        builder.Property(au => au.Language).IsRequired().HasMaxLength(10);
        builder.Property(au => au.DateNumberFormat).IsRequired().HasMaxLength(20);
        builder.Property(au => au.DefaultCurrency).IsRequired().HasMaxLength(10);
        builder.Property(au => au.Avatar).HasMaxLength(200);
        builder.Property(au => au.JobTitle).HasMaxLength(100);
        builder.Property(au => au.Mobile).HasMaxLength(15);
        builder.Property(au => au.IsActive).HasDefaultValue(true);
        //builder.Property(au => au.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP"); // SQL Server default value
        builder.Property(au => au.CreatedAt).HasDefaultValueSql("now()"); // PostgreSQL default value

        // Configure relationships
        builder.HasMany(au => au.TenantUsers).WithOne().HasForeignKey(tu => tu.ApplicationUserId).OnDelete(DeleteBehavior.Cascade);
    }
}