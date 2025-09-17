using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConnectFlow.Infrastructure.Data.Configurations;

public class NoteConfiguration : BaseAuditableConfiguration<Note>
{
    public override void Configure(EntityTypeBuilder<Note> builder)
    {
        base.Configure(builder);

        // Configure properties

        // Configure relationships
    }
}