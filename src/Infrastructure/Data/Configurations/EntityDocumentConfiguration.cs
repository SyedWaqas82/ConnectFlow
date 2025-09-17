using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class EntityDocumentConfiguration : BaseAuditableConfiguration<EntityDocument>
{
    public override void Configure(EntityTypeBuilder<EntityDocument> builder)
    {
        base.Configure(builder);

        //Configure properties

        // Configure relationships
    }
}