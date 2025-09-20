using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class EntityDocumentConfiguration : BaseAuditableConfiguration<EntityDocument>
{
    public override void Configure(EntityTypeBuilder<EntityDocument> builder)
    {
        base.Configure(builder);

        //Configure properties
        builder.Property(a => a.EntityId).IsRequired();
        builder.Property(a => a.EntityType).IsRequired();

        builder.HasIndex(a => new { a.TenantId, a.EntityType, a.EntityId }).HasDatabaseName("IX_Document_TenantId_EntityType_EntityId");

        // Configure relationships
    }
}