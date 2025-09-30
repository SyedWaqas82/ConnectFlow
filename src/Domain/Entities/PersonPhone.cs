namespace ConnectFlow.Domain.Entities;

public class PersonPhone : BaseAuditableEntity, ITenantableEntity
{
    public int PersonId { get; set; }
    public Person Person { get; set; } = null!;
    public string? CountryCode { get; set; }
    public required string PhoneNumber { get; set; }
    public string? Extension { get; set; }
    public PhoneType PhoneType { get; set; } = PhoneType.Work;
    public bool IsPrimary { get; set; } = false;
    public bool IsVerified { get; set; } = false;
    public DateTimeOffset? VerifiedAt { get; set; }

    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}