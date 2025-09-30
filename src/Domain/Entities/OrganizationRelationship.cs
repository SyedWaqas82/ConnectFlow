namespace ConnectFlow.Domain.Entities;

public class OrganizationRelationship : BaseAuditableEntity, ITenantableEntity
{
    public int PrimaryOrganizationId { get; set; }
    public Organization PrimaryOrganization { get; set; } = null!;
    public int RelatedOrganizationId { get; set; }
    public Organization RelatedOrganization { get; set; } = null!;
    public OrganizationRelationshipType RelationshipType { get; set; }


    // ITenantableEntity implementation
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}