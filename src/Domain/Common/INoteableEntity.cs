namespace ConnectFlow.Domain.Common;

public interface INoteableEntity
{
    int Id { get; }
    IList<Note> Notes { get; set; }
    EntityType EntityType { get; }
}