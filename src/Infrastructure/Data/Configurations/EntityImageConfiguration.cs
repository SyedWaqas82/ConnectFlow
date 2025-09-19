using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class EntityImageConfiguration : BaseAuditableConfiguration<EntityImage>
{
    public override void Configure(EntityTypeBuilder<EntityImage> builder)
    {
        base.Configure(builder);

        // Configure properties

        // Configure relationships
    }
}