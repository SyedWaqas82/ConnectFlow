using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class ProjectTaskConfiguration : BaseAuditableConfiguration<ProjectTask>
{
    public override void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(pt => pt.Title).IsRequired().HasMaxLength(200);
        builder.Property(pt => pt.Description).HasMaxLength(2000);
        builder.Property(pt => pt.SortOrder).IsRequired();
        builder.Property(pt => pt.Status).IsRequired().HasConversion<string>();

        // Configure relationships
        builder.HasOne(pt => pt.ProjectPhase).WithMany(pp => pp.ProjectTasks).HasForeignKey(pt => pt.ProjectPhaseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(pt => pt.Assignee).WithMany(tu => tu.AssignedProjectTasks).HasForeignKey(pt => pt.AssigneeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(pt => pt.ParentTask).WithMany(pt => pt.SubTasks).HasForeignKey(pt => pt.ParentTaskId).OnDelete(DeleteBehavior.Restrict);
    }
}