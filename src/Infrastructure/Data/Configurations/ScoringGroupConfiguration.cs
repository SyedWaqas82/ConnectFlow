using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class ScoringGroupConfiguration : BaseAuditableConfiguration<ScoringGroup>
{
    public override void Configure(EntityTypeBuilder<ScoringGroup> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(sg => sg.Name).IsRequired().HasMaxLength(200);
        builder.Property(sg => sg.Description).HasMaxLength(2000);
        builder.Property(sg => sg.Points).IsRequired();
        builder.Property(sg => sg.SortOrder).IsRequired();

        // Configure relationships
        builder.HasOne(sg => sg.ScoringProfile).WithMany(sp => sp.ScoringGroups).HasForeignKey(sg => sg.ScoringProfileId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(sg => sg.ScoringCriterias).WithOne(sc => sc.ScoringGroup).HasForeignKey(sc => sc.ScoringGroupId).OnDelete(DeleteBehavior.Cascade);
    }
}