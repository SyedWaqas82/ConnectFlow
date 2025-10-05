using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class PipelineConfiguration : BaseAuditableConfiguration<Pipeline>
{
    public override void Configure(EntityTypeBuilder<Pipeline> builder)
    {
        base.Configure(builder);

        // Configure Properties
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.SortOrder).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(500);

        // Configure relationships
        builder.HasOne(p => p.ScoringProfile).WithMany(sp => sp.Pipelines).HasForeignKey(p => p.ScoringProfileId).OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(p => p.Deals).WithOne(d => d.Pipeline).HasForeignKey(d => d.PipelineId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(p => p.Stages).WithOne(ps => ps.Pipeline).HasForeignKey(ps => ps.PipelineId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(p => p.StageHistories).WithOne(dsh => dsh.Pipeline).HasForeignKey(dsh => dsh.PipelineId).OnDelete(DeleteBehavior.Cascade);
    }
}