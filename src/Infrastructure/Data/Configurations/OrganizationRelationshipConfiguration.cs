using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class OrganizationRelationshipConfiguration : BaseAuditableConfiguration<OrganizationRelationship>
{
    public override void Configure(EntityTypeBuilder<OrganizationRelationship> builder)
    {
        base.Configure(builder);

        // Configure Properties
        builder.Property(a => a.RelationshipType).IsRequired().HasConversion<string>();

        // Configure relationships
        builder.HasOne(a => a.PrimaryOrganization).WithMany(o => o.PrimaryRelationships).HasForeignKey(a => a.PrimaryOrganizationId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(a => a.RelatedOrganization).WithMany(o => o.RelatedRelationships).HasForeignKey(a => a.RelatedOrganizationId).OnDelete(DeleteBehavior.Cascade);
    }
}