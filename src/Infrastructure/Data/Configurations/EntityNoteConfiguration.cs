using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class EntityNoteConfiguration : BaseAuditableConfiguration<EntityNote>
{
    public override void Configure(EntityTypeBuilder<EntityNote> builder)
    {
        base.Configure(builder);

        // Configure properties
        builder.Property(a => a.Content).IsRequired().HasMaxLength(4000);
        builder.Property(a => a.IsPinned).IsRequired();
        builder.Property(a => a.PinOrder).IsRequired();
        builder.Property(a => a.Type).IsRequired().HasConversion<string>();

        builder.Property(a => a.EntityId).IsRequired();
        builder.Property(a => a.EntityType).IsRequired().HasConversion<string>();

        builder.HasIndex(a => new { a.TenantId, a.EntityType, a.EntityId }).HasDatabaseName("IX_Note_TenantId_EntityType_EntityId");

        // Configure relationships
        builder.HasOne(a => a.Author).WithMany(tu => tu.Notes).HasForeignKey(a => a.AuthorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(a => a.Reactions).WithOne(nr => nr.Note).HasForeignKey(nr => nr.NoteId).OnDelete(DeleteBehavior.Cascade);
    }
}