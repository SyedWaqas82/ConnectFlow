using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class EntitySequenceEnrollmentConfiguration : BaseAuditableConfiguration<EntitySequenceEnrollment>
{
    public override void Configure(EntityTypeBuilder<EntitySequenceEnrollment> builder)
    {
        base.Configure(builder);

        // Configure Properties
        builder.Property(a => a.Status).IsRequired().HasConversion<string>();
        builder.Property(a => a.EnrolledAt).IsRequired();

        builder.Property(a => a.EntityId).IsRequired();
        builder.Property(a => a.EntityType).IsRequired().HasConversion<string>();

        builder.HasIndex(a => new { a.TenantId, a.EntityType, a.EntityId }).HasDatabaseName("IX_Entity_Sequence_Enrollment_TenantId_EntityType_EntityId");

        // Configure relationships
        builder.HasOne(e => e.CurrentStep).WithMany(ss => ss.CurrentEnrollments).HasForeignKey(e => e.CurrentStepId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(e => e.Sequence).WithMany(s => s.Enrollments).HasForeignKey(e => e.SequenceId).OnDelete(DeleteBehavior.Cascade);
    }
}