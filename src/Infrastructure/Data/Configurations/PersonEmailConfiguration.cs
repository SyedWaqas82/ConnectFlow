using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class PersonEmailConfiguration : BaseAuditableConfiguration<PersonEmail>
{
    public override void Configure(EntityTypeBuilder<PersonEmail> builder)
    {
        base.Configure(builder);

        // Configure Properties
        builder.Property(pe => pe.EmailAddress).IsRequired().HasMaxLength(256);
        builder.Property(pe => pe.EmailType).IsRequired().HasConversion<string>();

        // Configure relationships
        builder.HasOne(pe => pe.Person).WithMany(p => p.Emails).HasForeignKey(pe => pe.PersonId).OnDelete(DeleteBehavior.Cascade);
    }
}