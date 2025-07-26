using ConnectFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class StageConfiguration : BaseAuditableConfiguration<Stage>
{
    public override void Configure(EntityTypeBuilder<Stage> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Description).HasMaxLength(500);
        builder.Property(s => s.Order).IsRequired();

        builder.OwnsOne(b => b.Color);
        builder.Property(s => s.WinProbability).HasPrecision(5, 2).IsRequired();
        builder.Property(s => s.IsDefault).IsRequired();
        builder.Property(s => s.IsActive).IsRequired();

        // Configure relationships
        builder.HasOne(s => s.Pipeline).WithMany(p => p.Stages).HasForeignKey(s => s.PipelineId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(s => s.Leads).WithOne(l => l.Stage).HasForeignKey(l => l.StageId).OnDelete(DeleteBehavior.Restrict);
    }
}