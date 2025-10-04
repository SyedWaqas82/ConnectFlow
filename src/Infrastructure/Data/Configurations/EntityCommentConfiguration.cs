using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class EntityCommentConfiguration : BaseAuditableConfiguration<EntityComment>
{
    public override void Configure(EntityTypeBuilder<EntityComment> builder)
    {
        base.Configure(builder);

        //Configure properties
        builder.Property(a => a.EntityId).IsRequired();
        builder.Property(a => a.EntityType).IsRequired().HasConversion<string>();

        builder.HasIndex(a => new { a.TenantId, a.EntityType, a.EntityId }).HasDatabaseName("IX_Comment_TenantId_EntityType_EntityId");

        // Configure relationships
        builder.HasOne(a => a.Author).WithMany(tu => tu.Comments).HasForeignKey(a => a.AuthorId).OnDelete(DeleteBehavior.Restrict);
    }
}