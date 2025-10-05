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
    }
}