using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class EntityLabelConfiguration : BaseAuditableConfiguration<EntityLabel>
{
    public override void Configure(EntityTypeBuilder<EntityLabel> builder)
    {
        base.Configure(builder);

        // Configure properties


        // Configure relationships
    }
}