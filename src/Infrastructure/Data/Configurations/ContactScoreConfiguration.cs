using ConnectFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class ContactScoreConfiguration : BaseAuditableConfiguration<ContactScore>
{
    public override void Configure(EntityTypeBuilder<ContactScore> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(cs => cs.EngagementScore).IsRequired();
        builder.Property(cs => cs.QualificationScore).IsRequired();
        //builder.Property(cs => cs.TotalScore).IsRequired();

        // Configure relationships
        builder.HasOne(cs => cs.Contact).WithOne(c => c.Score).HasForeignKey<ContactScore>(cs => cs.ContactId).OnDelete(DeleteBehavior.Cascade);
    }
}