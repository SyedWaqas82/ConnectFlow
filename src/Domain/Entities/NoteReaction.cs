namespace ConnectFlow.Domain.Entities;

public class NoteReaction : BaseAuditableEntity
{
    public int EntityNoteId { get; set; }
    public EntityNote Note { get; set; } = null!;

    public int UserId { get; set; }
    public TenantUser User { get; set; } = null!;

    public ReactionType Type { get; set; } = ReactionType.Like;
}