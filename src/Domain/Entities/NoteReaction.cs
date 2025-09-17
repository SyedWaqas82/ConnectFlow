namespace ConnectFlow.Domain.Entities;

public class NoteReaction : BaseAuditableEntity
{
    public int NoteId { get; set; }
    public Note Note { get; set; } = null!;

    public int UserId { get; set; }
    public TenantUser User { get; set; } = null!;

    public ReactionType Type { get; set; } = ReactionType.Like;
}