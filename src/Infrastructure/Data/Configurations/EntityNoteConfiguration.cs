using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class EntityNoteConfiguration : BaseAuditableConfiguration<EntityNote>
{
    public override void Configure(EntityTypeBuilder<EntityNote> builder)
    {
        base.Configure(builder);

        // Configure properties

        // Configure relationships
    }
}