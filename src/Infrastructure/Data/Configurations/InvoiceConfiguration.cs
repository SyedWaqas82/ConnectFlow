using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class InvoiceConfiguration : BaseAuditableConfiguration<Invoice>
{
    public override void Configure(EntityTypeBuilder<Invoice> builder)
    {
        base.Configure(builder);

        builder.Property(i => i.PaymentProviderInvoiceId).IsRequired().HasMaxLength(50);
        builder.Property(i => i.Status).IsRequired().HasMaxLength(20);
        builder.Property(i => i.Amount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(i => i.Currency).IsRequired().HasMaxLength(3);
        builder.Property(i => i.PaidAt).IsRequired(false);

        builder.HasIndex(i => i.PaymentProviderInvoiceId).IsUnique();

        // Configure relationships
        builder.HasOne(i => i.Subscription).WithMany(s => s.Invoices).HasForeignKey(i => i.SubscriptionId).OnDelete(DeleteBehavior.Cascade);
    }
}