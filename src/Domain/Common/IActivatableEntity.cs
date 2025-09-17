namespace ConnectFlow.Domain.Common;

public interface IActivatableEntity
{
    int Id { get; }
    IList<EntityActivity> Activities { get; set; }
    EntityType EntityType { get; }
}