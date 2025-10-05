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

        // Configure Indexes

        // Foreign key index
        builder.HasIndex(pp => pp.ProjectBoardId).HasDatabaseName("IX_ProjectPhase_ProjectBoardId");

        // Name index for searches
        builder.HasIndex(pp => pp.Name).HasDatabaseName("IX_ProjectPhase_Name");

        // Sort order index for ordering
        builder.HasIndex(pp => pp.SortOrder).HasDatabaseName("IX_ProjectPhase_SortOrder");

        // Composite index for board + sort order (common query pattern)
        builder.HasIndex(pp => new { pp.ProjectBoardId, pp.SortOrder }).HasDatabaseName("IX_ProjectPhase_ProjectBoardId_SortOrder");
    }
}