// using ConnectFlow.Domain.Entities;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;

// namespace ConnectFlow.Infrastructure.Data.Configurations;

// public class CustomFieldConfiguration : BaseAuditableConfiguration<CustomField>
// {
//     public override void Configure(EntityTypeBuilder<CustomField> builder)
//     {
//         base.Configure(builder);

//         // Configure properties
//         builder.Property(cf => cf.Name).IsRequired().HasMaxLength(100);
//         builder.Property(cf => cf.Label).IsRequired().HasMaxLength(200);
//         builder.Property(cf => cf.Type).IsRequired();
//         builder.Property(cf => cf.EntityType).IsRequired();
//         builder.Property(cf => cf.Options).HasColumnType("jsonb"); // Assuming PostgreSQL for JSON support
//         builder.Property(cf => cf.IsRequired).IsRequired();
//         builder.Property(cf => cf.Order).IsRequired();
//         builder.Property(cf => cf.IsActive).IsRequired();

//         // Configure Tenant relationship
//     }
// }