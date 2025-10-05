using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class ProjectBoardConfiguration : BaseAuditableConfiguration<ProjectBoard>
{
    public override void Configure(EntityTypeBuilder<ProjectBoard> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(pb => pb.Name).IsRequired().HasMaxLength(200);
        builder.Property(pb => pb.SortOrder).IsRequired();
        builder.Property(pb => pb.Description).HasMaxLength(1000);

        // Configure relationships
        builder.HasMany(pb => pb.Projects).WithOne(p => p.ProjectBoard).HasForeignKey(p => p.ProjectBoardId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(pb => pb.Phases).WithOne(pp => pp.ProjectBoard).HasForeignKey(pp => pp.ProjectBoardId).OnDelete(DeleteBehavior.Restrict);
    }
}