namespace ConnectFlow.Domain.Common;

public interface ILabelableEntity
{
    int Id { get; }
    IList<EntityLabel> Labels { get; set; }
    EntityType EntityType { get; }
}