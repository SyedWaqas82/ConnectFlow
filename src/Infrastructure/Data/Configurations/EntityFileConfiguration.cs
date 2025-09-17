using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class EntityFileConfiguration : BaseAuditableConfiguration<EntityFile>
{
    public override void Configure(EntityTypeBuilder<EntityFile> builder)
    {
        base.Configure(builder);

        //Configure properties

        // Configure relationships
    }
}