using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class ProjectConfiguration : BaseAuditableConfiguration<Project>
{
    public override void Configure(EntityTypeBuilder<Project> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Title).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(2000);
        builder.Property(p => p.Status).IsRequired().HasConversion<string>();
        builder.Property(p => p.Priority).IsRequired().HasConversion<string>();

        // Configure relationships
        builder.HasOne(p => p.Owner).WithMany(tu => tu.Projects).HasForeignKey(p => p.OwnerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.Organization).WithMany(o => o.Projects).HasForeignKey(p => p.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.Person).WithMany(pers => pers.Projects).HasForeignKey(p => p.PersonId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.ProjectBoard).WithMany(pb => pb.Projects).HasForeignKey(p => p.ProjectBoardId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.ProjectPhase).WithMany(pp => pp.Projects).HasForeignKey(p => p.ProjectPhaseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(p => p.ProjectDeals).WithOne(pd => pd.Project).HasForeignKey(pd => pd.ProjectId).OnDelete(DeleteBehavior.Cascade);

        // Configure Indexes

        // Foreign key indexes for common joins
        builder.HasIndex(p => p.OwnerId).HasDatabaseName("IX_Project_OwnerId");
        builder.HasIndex(p => p.OrganizationId).HasDatabaseName("IX_Project_OrganizationId");
        builder.HasIndex(p => p.PersonId).HasDatabaseName("IX_Project_PersonId");
        builder.HasIndex(p => p.ProjectBoardId).HasDatabaseName("IX_Project_ProjectBoardId");
        builder.HasIndex(p => p.ProjectPhaseId).HasDatabaseName("IX_Project_ProjectPhaseId");

        // Common filter indexes
        builder.HasIndex(p => p.Status).HasDatabaseName("IX_Project_Status");
        builder.HasIndex(p => p.Priority).HasDatabaseName("IX_Project_Priority");

        // Composite index for common filtering scenarios
        builder.HasIndex(p => new { p.Status, p.Priority }).HasDatabaseName("IX_Project_Status_Priority");
    }
}