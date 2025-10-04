using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class DealInstallmentConfiguration : BaseAuditableConfiguration<DealInstallment>
{
    public override void Configure(EntityTypeBuilder<DealInstallment> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(di => di.Description).IsRequired().HasMaxLength(500);
        builder.Property(di => di.BillingDate).IsRequired();
        builder.Property(di => di.Amount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(di => di.SortOrder).IsRequired();

        // Configure relationships
        builder.HasOne(di => di.Deal).WithMany(d => d.Installments).HasForeignKey(di => di.DealId).OnDelete(DeleteBehavior.Cascade);
    }
}