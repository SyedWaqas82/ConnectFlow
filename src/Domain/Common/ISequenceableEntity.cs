namespace ConnectFlow.Domain.Common;

public interface ISequenceableEntity
{
    int Id { get; }
    IList<EntitySequenceEnrollment> SequenceEnrollments { get; set; }
    EntityType EntityType { get; }
}