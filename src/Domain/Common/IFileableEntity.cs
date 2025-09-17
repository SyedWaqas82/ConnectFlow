namespace ConnectFlow.Domain.Common;

public interface IFileableEntity
{
    int Id { get; }
    IList<EntityFile> Files { get; set; }
    EntityType EntityType { get; }
}