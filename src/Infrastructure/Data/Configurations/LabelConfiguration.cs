using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class LabelConfiguration : BaseAuditableConfiguration<Label>
{
    public override void Configure(EntityTypeBuilder<Label> builder)
    {
        base.Configure(builder);

        // Configure Properties
        builder.Property(a => a.Name).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Color).IsRequired().HasMaxLength(7); // Hex color code length
        builder.Property(a => a.Description).HasMaxLength(500);
        builder.Property(a => a.SortOrder).IsRequired();
        builder.Property(a => a.EntityType).IsRequired().HasConversion<string>();

        // Configure relationships
        builder.HasMany(a => a.Labels).WithOne(el => el.Label).HasForeignKey(el => el.LabelId).OnDelete(DeleteBehavior.Cascade);
    }
}