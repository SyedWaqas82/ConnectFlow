using System.Text.Json;

namespace ConnectFlow.Domain.Entities;

public class TenantUser : BaseAuditableEntity, ISuspendibleEntity
{
    public int ApplicationUserId { get; set; } = default!;
    public TenantUserStatus Status { get; set; } = TenantUserStatus.Active;
    public DateTimeOffset JoinedAt { get; set; }
    public DateTimeOffset? LeftAt { get; set; }
    public int? InvitedBy { get; set; } // ApplicationUserId of inviter
    public string TimeZone { get; set; } = "UTC";
    public string Language { get; set; } = "en";
    public string DateNumberFormat { get; set; } = "MM/dd/yyyy";
    public string DefaultCurrency { get; set; } = "USD";
    public string? Settings { get; set; } = JsonSerializer.Serialize(new
    {
        Activity = new { ShowModelAfterWinningDeal = true },
        Deal = new { AutoAddDealToTitle = false, ShowCelebrationAnimation = true },
        Notification = new { EmailNotifications = true, PushNotifications = false }, // Example settings
        UI = new { Theme = "Light", DefaultLandingPage = "Dashboard" },
        Sidebar = new { ShowItems = new[] { "Dashboard", "Leads", "Deals", "Contacts", "Activities" }, MoreItems = new[] { "Projects", "Products", "Reports" } }
    });

    // ISuspendibleEntity implementation
    public EntityStatus EntityStatus { get; set; } = EntityStatus.Active;
    public DateTimeOffset? SuspendedAt { get; set; }
    public DateTimeOffset? ResumedAt { get; set; }

    // Navigation properties
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public IList<TenantUserRole> TenantUserRoles { get; private set; } = new List<TenantUserRole>();
    public IList<Lead> Leads { get; private set; } = new List<Lead>(); // Leads owned by this user
    public IList<Deal> Deals { get; private set; } = new List<Deal>(); // Deals owned by this user
    public IList<Organization> Organizations { get; private set; } = new List<Organization>(); // Organizations owned by this user
    public IList<Person> People { get; private set; } = new List<Person>(); // People owned by this user
    public IList<EntityNote> Notes { get; private set; } = new List<EntityNote>(); // Authored notes
    public IList<EntityActivity> Activities { get; private set; } = new List<EntityActivity>(); // Activities assigned to this user
    public IList<Project> Projects { get; private set; } = new List<Project>(); // Projects owned by this user
    public IList<Product> Products { get; private set; } = new List<Product>(); // Products owned by this user
    public IList<Scheduler> Schedulers { get; private set; } = new List<Scheduler>(); // Schedulers owned by this user
    public IList<AssignmentRule> AssignmentRules { get; private set; } = new List<AssignmentRule>(); // Assignment rules where this user is assigned tasks
    public IList<AssignmentRuleHistory> PreviousAssignmentRuleHistories { get; private set; } = new List<AssignmentRuleHistory>(); // Assignment rule histories where this user was previously assigned
    public IList<AssignmentRuleHistory> NewAssignmentRuleHistories { get; private set; } = new List<AssignmentRuleHistory>(); // Assignment rule histories where this user is newly assigned
    public IList<AssignmentRuleHistory> TriggeredAssignmentRuleHistories { get; private set; } = new List<AssignmentRuleHistory>(); // Assignment rule histories triggered by this user
    public IList<EntityActivity> AssignedByActivities { get; private set; } = new List<EntityActivity>(); // Activities assigned by this user
    public IList<EntityActivity> AssignedActivities { get; private set; } = new List<EntityActivity>(); // Activities assigned to this user
    public IList<EntityComment> Comments { get; private set; } = new List<EntityComment>(); // Comments authored by this user
    public IList<EntityLabel> AssignedByLabels { get; private set; } = new List<EntityLabel>(); // Labels assigned by this user
    public IList<NoteReaction> NoteReactions { get; private set; } = new List<NoteReaction>(); // Note reactions by this user
    public IList<ProjectTask> AssignedProjectTasks { get; private set; } = new List<ProjectTask>(); // Project tasks assigned to this user
    public IList<Sequence> Sequences { get; private set; } = new List<Sequence>(); // Sequences owned by this user
}