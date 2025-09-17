using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class EntityCommentConfiguration : BaseAuditableConfiguration<EntityComment>
{
    public override void Configure(EntityTypeBuilder<EntityComment> builder)
    {
        base.Configure(builder);

        //Configure properties

        // Configure relationships
    }
}