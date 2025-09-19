namespace ConnectFlow.Domain.Common;

public interface IPriceableEntity
{
    int Id { get; }
    IList<EntityPrice> Prices { get; set; }
    EntityType EntityType { get; }
}