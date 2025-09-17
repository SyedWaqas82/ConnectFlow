namespace ConnectFlow.Domain.Common;

public interface ICommentableEntity
{
    int Id { get; }
    IList<EntityComment> Comments { get; set; }
    EntityType EntityType { get; }
}