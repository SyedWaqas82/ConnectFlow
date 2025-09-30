namespace ConnectFlow.Domain.Common;

public interface IParticipantable
{
    int Id { get; }
    IList<EntityParticipant> Participants { get; set; }
    EntityType EntityType { get; }
}