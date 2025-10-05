using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class ProjectPhaseConfiguration : BaseAuditableConfiguration<ProjectPhase>
{
    public override void Configure(EntityTypeBuilder<ProjectPhase> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(pp => pp.Name).IsRequired().HasMaxLength(200);
        builder.Property(pp => pp.SortOrder).IsRequired();

        // Configure relationships
        builder.HasOne(pp => pp.ProjectBoard).WithMany(pb => pb.Phases).HasForeignKey(pp => pp.ProjectBoardId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(pp => pp.Projects).WithOne(p => p.ProjectPhase).HasForeignKey(p => p.ProjectPhaseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(pp => pp.ProjectTasks).WithOne(t => t.ProjectPhase).HasForeignKey(t => t.ProjectPhaseId).OnDelete(DeleteBehavior.Restrict);
    }
}