using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class LeadConfiguration : BaseAuditableConfiguration<Lead>
{
    public override void Configure(EntityTypeBuilder<Lead> builder)
    {
        base.Configure(builder);

        // Configure Properties
        builder.Property(a => a.Title).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Value).HasColumnType("decimal(18,2)");
        builder.Property(a => a.Currency).IsRequired().HasMaxLength(3);
        builder.Property(a => a.SourceOrigin).HasMaxLength(200);
        builder.Property(a => a.SourceChannel).IsRequired().HasConversion<string>();
        builder.Property(a => a.SourceChannelId).HasMaxLength(100);

        // Configure relationships
        builder.HasOne(a => a.Owner).WithMany(tu => tu.Leads).HasForeignKey(a => a.OwnerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.Person).WithMany(p => p.Leads).HasForeignKey(a => a.PersonId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(a => a.Organization).WithMany(o => o.Leads).HasForeignKey(a => a.OrganizationId).OnDelete(DeleteBehavior.SetNull);
    }
}