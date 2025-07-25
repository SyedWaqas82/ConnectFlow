using ConnectFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class LeadConfiguration : BaseAuditableConfiguration<Lead>
{
    public override void Configure(EntityTypeBuilder<Lead> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(l => l.Status).IsRequired();
        builder.Property(l => l.Title).IsRequired().HasMaxLength(200);
        builder.Property(l => l.Description).IsRequired().HasMaxLength(1000);
        builder.Property(l => l.Value).HasColumnType("decimal(18,2)");
        builder.Property(l => l.Currency).HasMaxLength(10).HasDefaultValue("USD");
        builder.Property(l => l.ExpectedCloseDate).HasColumnType("datetimeoffset");

        // Configure relationships
        builder.HasOne(l => l.Contact).WithMany(c => c.Leads).HasForeignKey(l => l.ContactId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(l => l.Company).WithMany(c => c.Leads).HasForeignKey(l => l.CompanyId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(l => l.Stage).WithMany(s => s.Leads).HasForeignKey(l => l.StageId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(l => l.Pipeline).WithMany(p => p.Leads).HasForeignKey(l => l.PipelineId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(l => l.Owner).WithMany(u => u.Leads).HasForeignKey(l => l.OwnerId).OnDelete(DeleteBehavior.Restrict);
    }
}