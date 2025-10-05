using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class OrganizationConfiguration : BaseAuditableConfiguration<Organization>
{
    public override void Configure(EntityTypeBuilder<Organization> builder)
    {
        base.Configure(builder);

        // Configure Properties
        builder.Property(a => a.Name).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Address).HasMaxLength(500);
        builder.Property(a => a.Website).HasMaxLength(200);
        builder.Property(a => a.LinkedIn).HasMaxLength(200);
        builder.Property(a => a.Industry).HasMaxLength(100);
        builder.Property(a => a.NumberOfEmployees);
        builder.Property(a => a.AnnualRevenue).HasColumnType("decimal(18,2)");

        // Configure relationships
        builder.HasOne(a => a.Owner).WithMany(tu => tu.Organizations).HasForeignKey(a => a.OwnerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(o => o.Deals).WithOne(d => d.Organization).HasForeignKey(d => d.OrganizationId).OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(o => o.People).WithOne(p => p.Organization).HasForeignKey(p => p.OrganizationId).OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(o => o.Leads).WithOne(l => l.Organization).HasForeignKey(l => l.OrganizationId).OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(o => o.Projects).WithOne(p => p.Organization).HasForeignKey(p => p.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(o => o.PrimaryRelationships).WithOne(or => or.PrimaryOrganization).HasForeignKey(or => or.PrimaryOrganizationId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(o => o.RelatedRelationships).WithOne(or => or.RelatedOrganization).HasForeignKey(or => or.RelatedOrganizationId).OnDelete(DeleteBehavior.Cascade);
    }
}