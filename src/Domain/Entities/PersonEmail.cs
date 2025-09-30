namespace ConnectFlow.Domain.Entities;

public class PersonEmail : BaseAuditableEntity, ITenantableEntity
{
    public int PersonId { get; set; }
    public Person Person { get; set; } = null!;
    public required string EmailAddress { get; set; }
    public EmailType EmailType { get; set; } = EmailType.Work;
    public bool IsPrimary { get; set; } = false;
    public bool IsVerified { get; set; } = false;
    public DateTimeOffset? VerifiedAt { get; set; }

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}