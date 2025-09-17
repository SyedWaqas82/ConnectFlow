namespace ConnectFlow.Domain.Common;

public interface IDocumentableEntity
{
    int Id { get; }
    IList<EntityDocument> Documents { get; set; }
    EntityType EntityType { get; }
}