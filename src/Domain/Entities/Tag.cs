namespace ConnectFlow.Domain.Entities;

public class Tag : BaseAuditableEntity, ITenantEntity
{
    public string Name { get; set; } = string.Empty;
    public Colour Color { get; set; } = Colour.Green;
    public string Description { get; set; } = string.Empty;

    public IList<Task> Tasks { get; private set; } = new List<Task>();
    public IList<Conversation> Conversations { get; private set; } = new List<Conversation>();
    public IList<Contact> Contacts { get; private set; } = new List<Contact>();
    public IList<Company> Companies { get; private set; } = new List<Company>();

    // Tenant
    public int TenantId { get; set; } = default!;
    public Tenant Tenant { get; set; } = null!;
}
