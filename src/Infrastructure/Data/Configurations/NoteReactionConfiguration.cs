using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class NoteReactionConfiguration : BaseAuditableConfiguration<NoteReaction>
{
    public override void Configure(EntityTypeBuilder<NoteReaction> builder)
    {
        base.Configure(builder);

        // Configure Properties
        builder.Property(a => a.Type).IsRequired().HasConversion<string>();

        // Configure relationships
        builder.HasOne(a => a.Note).WithMany(n => n.Reactions).HasForeignKey(a => a.NoteId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(a => a.User).WithMany(tu => tu.NoteReactions).HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}