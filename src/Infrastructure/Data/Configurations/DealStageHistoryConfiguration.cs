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
    }
}