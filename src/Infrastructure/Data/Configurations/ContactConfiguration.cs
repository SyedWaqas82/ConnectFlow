using ConnectFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class ContactConfiguration : BaseAuditableConfiguration<Contact>
{
    public override void Configure(EntityTypeBuilder<Contact> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.LastName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Phone).HasMaxLength(20);
        builder.Property(c => c.JobTitle).HasMaxLength(100);


        // Configure relationships
        builder.HasOne(c => c.Company).WithMany(co => co.Contacts).HasForeignKey(c => c.CompanyId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(c => c.Appointments).WithOne(a => a.Contact).HasForeignKey(a => a.ContactId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(c => c.Leads).WithOne(l => l.Contact).HasForeignKey(l => l.ContactId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(c => c.Score).WithOne(cs => cs.Contact).HasForeignKey<Contact>(c => c.ScoreId).OnDelete(DeleteBehavior.Restrict);
    }
}