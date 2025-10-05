using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class ScoringProfileConfiguration : BaseAuditableConfiguration<ScoringProfile>
{
    public override void Configure(EntityTypeBuilder<ScoringProfile> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(sp => sp.Name).IsRequired().HasMaxLength(200);
        builder.Property(sp => sp.Description).HasMaxLength(2000);
        builder.Property(sp => sp.TargetEntityType).IsRequired().HasConversion<string>();
        builder.Property(sp => sp.MaxScore).IsRequired();
        builder.Property(sp => sp.MinScore).IsRequired();

        // Configure relationships
        builder.HasMany(sp => sp.ScoringGroups).WithOne(sg => sg.ScoringProfile).HasForeignKey(sg => sg.ScoringProfileId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(sp => sp.Pipelines).WithOne(p => p.ScoringProfile).HasForeignKey(p => p.ScoringProfileId).OnDelete(DeleteBehavior.SetNull);
    }
}