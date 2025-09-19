namespace ConnectFlow.Domain.Common;

public interface IImageableEntity
{
    int Id { get; }
    IList<EntityImage> Images { get; set; }
    EntityType EntityType { get; }
}