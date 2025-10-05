using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class ProjectDealConfiguration : BaseAuditableConfiguration<ProjectDeal>
{
    public override void Configure(EntityTypeBuilder<ProjectDeal> builder)
    {
        base.Configure(builder);

        // Configure properties

        // Configure relationships
        builder.HasOne(pd => pd.Project).WithMany(p => p.ProjectDeals).HasForeignKey(pd => pd.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(pd => pd.Deal).WithMany(d => d.ProjectDeals).HasForeignKey(pd => pd.DealId).OnDelete(DeleteBehavior.Cascade);
    }
}