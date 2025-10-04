using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class EntityActivityParticipantConfiguration : BaseAuditableConfiguration<EntityActivityParticipant>
{
    public override void Configure(EntityTypeBuilder<EntityActivityParticipant> builder)
    {
        base.Configure(builder);

        // Configure properties

        // Configure relationships
        builder.HasOne(p => p.Person).WithMany(p => p.ParticipatingActivities).HasForeignKey(p => p.PersonId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(p => p.Activity).WithMany(a => a.Participants).HasForeignKey(p => p.ActivityId).OnDelete(DeleteBehavior.Cascade);
    }
}