using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class PersonConfiguration : BaseAuditableConfiguration<Person>
{
    public override void Configure(EntityTypeBuilder<Person> builder)
    {
        base.Configure(builder);

        // Configure Properties
        builder.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.LastName).IsRequired().HasMaxLength(100);

        // Configure relationships
        builder.HasOne(p => p.Organization).WithMany(o => o.People).HasForeignKey(p => p.OrganizationId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(p => p.Owner).WithMany(u => u.People).HasForeignKey(p => p.OwnerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(p => p.Phones).WithOne(pp => pp.Person).HasForeignKey(pp => pp.PersonId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.Emails).WithOne(pe => pe.Person).HasForeignKey(pe => pe.PersonId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.ParticipatingActivities).WithOne(eap => eap.Person).HasForeignKey(eap => eap.PersonId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.Participants).WithOne(ep => ep.Person).HasForeignKey(ep => ep.PersonId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(p => p.Leads).WithOne(l => l.Person).HasForeignKey(l => l.PersonId).OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(p => p.Deals).WithOne(d => d.Person).HasForeignKey(d => d.PersonId).OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(p => p.Projects).WithOne(pr => pr.Person).HasForeignKey(pr => pr.PersonId).OnDelete(DeleteBehavior.Restrict);
    }
}