using ConnectFlow.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasIndex(u => u.PublicId).IsUnique();
        //builder.Property(t => t.PublicId).HasDefaultValueSql("newid()").IsRequired(); // SQL Server
        builder.Property(t => t.PublicId).HasDefaultValueSql("gen_random_uuid()").IsRequired(); // PostgreSQL
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(50);
        builder.Property(u => u.TimeZone).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Locale).IsRequired().HasMaxLength(10);
        builder.Property(u => u.Avatar).HasMaxLength(200);
        builder.Property(u => u.JobTitle).HasMaxLength(100);
        builder.Property(u => u.Mobile).HasMaxLength(15);
        builder.Property(u => u.Preferences).HasColumnType("jsonb"); // Store preferences as JSON
        builder.Property(u => u.IsActive).HasDefaultValue(true);
        //builder.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP"); // SQL Server default value
        builder.Property(u => u.CreatedAt).HasDefaultValueSql("now()"); // PostgreSQL default value

        // Configure relationships
        builder.HasMany(t => t.TenantUsers).WithOne().HasForeignKey(tu => tu.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}