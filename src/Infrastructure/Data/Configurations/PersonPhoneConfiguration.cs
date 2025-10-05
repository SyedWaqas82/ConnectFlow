using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class PersonPhoneConfiguration : BaseAuditableConfiguration<PersonPhone>
{
    public override void Configure(EntityTypeBuilder<PersonPhone> builder)
    {
        base.Configure(builder);

        // Configure Properties
        builder.Property(pp => pp.CountryCode).HasMaxLength(5);
        builder.Property(pp => pp.PhoneNumber).IsRequired().HasMaxLength(20);
        builder.Property(pp => pp.Extension).HasMaxLength(10);
        builder.Property(pp => pp.PhoneType).IsRequired().HasConversion<string>();

        // Configure relationships
        builder.HasOne(pp => pp.Person).WithMany(p => p.Phones).HasForeignKey(pp => pp.PersonId).OnDelete(DeleteBehavior.Cascade);
    }
}