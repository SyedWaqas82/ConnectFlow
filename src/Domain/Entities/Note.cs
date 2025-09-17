using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectFlow.Domain.Entities;

public class Note : BaseAuditableEntity, ITenantableEntity, ISoftDeleteableEntity, ICommentableEntity
{
    public required string Content { get; set; }
    public bool IsPinned { get; set; } = false;
    public int PinOrder { get; set; } = 0;
    public NoteType Type { get; set; } = NoteType.General;

    // Author information
    public int AuthorId { get; set; }
    public TenantUser Author { get; set; } = null!;

    // ITenantEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    // ISoftDeleteEntity implementation
    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }

    [NotMapped]
    public EntityType EntityType => EntityType.Note;
    [NotMapped]
    public IList<EntityComment> Comments { get; set; } = new List<EntityComment>();
    public IList<NoteReaction> Reactions { get; private set; } = new List<NoteReaction>();
}