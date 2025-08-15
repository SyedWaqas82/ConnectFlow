// using ConnectFlow.Domain.Entities;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;

// namespace ConnectFlow.Infrastructure.Data.Configurations;

// public class CompanyConfiguration : BaseAuditableConfiguration<Company>
// {
//     public override void Configure(EntityTypeBuilder<Company> builder)
//     {
//         base.Configure(builder);

//         // Configure properties
//         builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
//         builder.Property(c => c.Website).HasMaxLength(200);
//         builder.Property(c => c.Industry).HasMaxLength(100);

//         // Configure relationships
//         builder.HasMany(c => c.Contacts).WithOne(co => co.Company).HasForeignKey(co => co.CompanyId).OnDelete(DeleteBehavior.Restrict);
//         builder.HasMany(c => c.Leads).WithOne(l => l.Company).HasForeignKey(l => l.CompanyId).OnDelete(DeleteBehavior.Restrict);
//     }
// }