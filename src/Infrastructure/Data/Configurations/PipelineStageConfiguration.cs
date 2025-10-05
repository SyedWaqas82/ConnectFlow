using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class PipelineStageConfiguration : BaseAuditableConfiguration<PipelineStage>
{
    public override void Configure(EntityTypeBuilder<PipelineStage> builder)
    {
        base.Configure(builder);

        // Configure Properties
        builder.Property(ps => ps.Name).IsRequired().HasMaxLength(100);
        builder.Property(ps => ps.SortOrder).IsRequired();

        // Configure relationships
        builder.HasOne(ps => ps.Pipeline).WithMany(p => p.Stages).HasForeignKey(ps => ps.PipelineId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(ps => ps.Deals).WithOne(d => d.PipelineStage).HasForeignKey(d => d.PipelineStageId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(ps => ps.StageHistories).WithOne(sh => sh.PipelineStage).HasForeignKey(sh => sh.PipelineStageId).OnDelete(DeleteBehavior.Cascade);

        // Configure Indexes

        // Foreign key index
        builder.HasIndex(ps => ps.PipelineId).HasDatabaseName("IX_PipelineStage_PipelineId");

        // Name index for searches
        builder.HasIndex(ps => ps.Name).HasDatabaseName("IX_PipelineStage_Name");

        // Sort order index for ordering
        builder.HasIndex(ps => ps.SortOrder).HasDatabaseName("IX_PipelineStage_SortOrder");

        // Composite index for pipeline + sort order (common query pattern)
        builder.HasIndex(ps => new { ps.PipelineId, ps.SortOrder }).HasDatabaseName("IX_PipelineStage_PipelineId_SortOrder");
    }
}