namespace ConnectFlow.Domain.Common;

public interface IDealable
{
    int Id { get; }
    IList<EntityDeal> Deals { get; set; }
    EntityType EntityType { get; }
}