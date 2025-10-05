using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class DealStageHistoryConfiguration : BaseAuditableConfiguration<DealStageHistory>
{
    public override void Configure(EntityTypeBuilder<DealStageHistory> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(dsh => dsh.EnteredAt).IsRequired();

        // Configure relationships
        builder.HasOne(dsh => dsh.Deal).WithMany(d => d.StageHistories).HasForeignKey(dsh => dsh.DealId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(dsh => dsh.PipelineStage).WithMany(ps => ps.StageHistories).HasForeignKey(dsh => dsh.PipelineStageId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(dsh => dsh.Pipeline).WithMany(p => p.StageHistories).HasForeignKey(dsh => dsh.PipelineId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(dsh => dsh.PreviousStage).WithMany().HasForeignKey(dsh => dsh.PreviousStageId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(dsh => dsh.NextStage).WithMany().HasForeignKey(dsh => dsh.NextStageId).OnDelete(DeleteBehavior.SetNull);

        // Configure Indexes

        // Foreign key indexes
        builder.HasIndex(dsh => dsh.DealId).HasDatabaseName("IX_DealStageHistory_DealId");
        builder.HasIndex(dsh => dsh.PipelineStageId).HasDatabaseName("IX_DealStageHistory_PipelineStageId");
        builder.HasIndex(dsh => dsh.PipelineId).HasDatabaseName("IX_DealStageHistory_PipelineId");
        builder.HasIndex(dsh => dsh.PreviousStageId).HasDatabaseName("IX_DealStageHistory_PreviousStageId");
        builder.HasIndex(dsh => dsh.NextStageId).HasDatabaseName("IX_DealStageHistory_NextStageId");

        // Time-based index
        builder.HasIndex(dsh => dsh.EnteredAt).HasDatabaseName("IX_DealStageHistory_EnteredAt");

        // Composite index for common query pattern (timeline of a deal's progression)
        builder.HasIndex(dsh => new { dsh.DealId, dsh.EnteredAt }).HasDatabaseName("IX_DealStageHistory_DealId_EnteredAt");
    }
}