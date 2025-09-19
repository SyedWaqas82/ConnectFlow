using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class EntityPriceConfiguration : BaseAuditableConfiguration<EntityPrice>
{
    public override void Configure(EntityTypeBuilder<EntityPrice> builder)
    {
        base.Configure(builder);

        // Configure properties

        // Configure relationships
    }
}