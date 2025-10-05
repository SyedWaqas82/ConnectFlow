using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class DealConfiguration : BaseAuditableConfiguration<Deal>
{
    public override void Configure(EntityTypeBuilder<Deal> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(d => d.Title).IsRequired().HasMaxLength(200);
        builder.Property(d => d.Value).HasColumnType("decimal(18,2)");
        builder.Property(d => d.Currency).IsRequired().HasMaxLength(3);
        builder.Property(d => d.TaxType).IsRequired().HasConversion<string>();
        builder.Property(d => d.Probability).IsRequired();
        builder.Property(d => d.Score).IsRequired();
        builder.Property(d => d.ScorePercentage).HasColumnType("decimal(5,2)");
        builder.Property(d => d.SourceOrigin).HasMaxLength(200);
        builder.Property(d => d.SourceChannel).IsRequired().HasConversion<string>();
        builder.Property(d => d.SourceChannelId).HasMaxLength(100);
        builder.Property(d => d.Status).IsRequired().HasConversion<string>();
        builder.Property(d => d.WonLossReason).HasMaxLength(500);

        // Add indexes for commonly queried foreign keys
        builder.HasIndex(d => d.OwnerId).HasDatabaseName("IX_Deal_OwnerId");
        builder.HasIndex(d => d.PersonId).HasDatabaseName("IX_Deal_PersonId");
        builder.HasIndex(d => d.OrganizationId).HasDatabaseName("IX_Deal_OrganizationId");
        builder.HasIndex(d => d.PipelineStageId).HasDatabaseName("IX_Deal_PipelineStageId");
        builder.HasIndex(d => d.PipelineId).HasDatabaseName("IX_Deal_PipelineId");

        // Add composite indexes for common filtering scenarios
        builder.HasIndex(d => new { d.TenantId, d.Status }).HasDatabaseName("IX_Deal_TenantId_Status");
        builder.HasIndex(d => new { d.TenantId, d.OwnerId, d.Status }).HasDatabaseName("IX_Deal_TenantId_OwnerId_Status");
        builder.HasIndex(d => new { d.TenantId, d.PipelineId, d.PipelineStageId }).HasDatabaseName("IX_Deal_TenantId_PipelineId_StageId");

        // Configure relationships
        builder.HasOne(d => d.Owner).WithMany(tu => tu.Deals).HasForeignKey(d => d.OwnerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(d => d.Person).WithMany(p => p.Deals).HasForeignKey(d => d.PersonId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(d => d.Organization).WithMany(o => o.Deals).HasForeignKey(d => d.OrganizationId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(d => d.Lead).WithOne(l => l.Deal).HasForeignKey<Deal>(d => d.LeadId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(d => d.PipelineStage).WithMany(ps => ps.Deals).HasForeignKey(d => d.PipelineStageId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(d => d.Pipeline).WithMany(p => p.Deals).HasForeignKey(d => d.PipelineId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(d => d.DealProducts).WithOne(dp => dp.Deal).HasForeignKey(dp => dp.DealId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(d => d.ProjectDeals).WithOne(pd => pd.Deal).HasForeignKey(pd => pd.DealId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(d => d.Installments).WithOne(di => di.Deal).HasForeignKey(di => di.DealId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(d => d.StageHistories).WithOne(dsh => dsh.Deal).HasForeignKey(dsh => dsh.DealId).OnDelete(DeleteBehavior.Cascade);
    }
}