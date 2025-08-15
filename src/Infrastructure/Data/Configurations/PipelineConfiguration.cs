// using ConnectFlow.Domain.Entities;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;

// namespace ConnectFlow.Infrastructure.Data.Configurations;

// public class PipelineConfiguration : BaseAuditableConfiguration<Pipeline>
// {
//     public override void Configure(EntityTypeBuilder<Pipeline> builder)
//     {
//         base.Configure(builder);

//         // Configure properties
//         builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
//         builder.Property(p => p.Description).HasMaxLength(500);

//         // Configure relationships
//         builder.HasMany(p => p.Stages).WithOne(s => s.Pipeline).HasForeignKey(s => s.PipelineId).OnDelete(DeleteBehavior.Restrict);
//         builder.HasMany(p => p.Leads).WithOne(l => l.Pipeline).HasForeignKey(l => l.PipelineId).OnDelete(DeleteBehavior.Restrict);
//     }
// }