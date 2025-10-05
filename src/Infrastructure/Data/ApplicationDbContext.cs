using System.Reflection;
using ConnectFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ConnectFlow.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>, IApplicationDbContext
{
    private readonly IServiceProvider _serviceProvider;
    private IContextManager? _contextManager;

    public ApplicationDbContext(IServiceProvider serviceProvider, DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        _serviceProvider = serviceProvider;
    }

    private IContextManager ContextManager => _contextManager ??= _serviceProvider.GetRequiredService<IContextManager>();

    // Tenant Management
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantUser> TenantUsers => Set<TenantUser>();
    public DbSet<TenantUserRole> TenantUserRoles => Set<TenantUserRole>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<ChannelAccount> ChannelAccounts => Set<ChannelAccount>();

    // Activity Tracking
    public DbSet<EntityActivity> Activities => Set<EntityActivity>();
    public DbSet<EntityChangeLog> ChangeLogs => Set<EntityChangeLog>();

    // CRM - Deals
    public DbSet<Deal> Deals => Set<Deal>();
    public DbSet<DealProduct> DealProducts => Set<DealProduct>();
    public DbSet<DealStageHistory> DealStageHistories => Set<DealStageHistory>();
    public DbSet<DealInstallment> DealInstallments => Set<DealInstallment>();

    // CRM - Contacts
    public DbSet<Person> People => Set<Person>();
    public DbSet<PersonEmail> PersonEmails => Set<PersonEmail>();
    public DbSet<PersonPhone> PersonPhones => Set<PersonPhone>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<OrganizationRelationship> OrganizationRelationships => Set<OrganizationRelationship>();
    public DbSet<Lead> Leads => Set<Lead>();

    // Project Management
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectBoard> ProjectBoards => Set<ProjectBoard>();
    public DbSet<ProjectPhase> ProjectPhases => Set<ProjectPhase>();
    public DbSet<ProjectTask> ProjectTasks => Set<ProjectTask>();
    public DbSet<ProjectDeal> ProjectDeals => Set<ProjectDeal>();

    // Sales Pipeline
    public DbSet<Pipeline> Pipelines => Set<Pipeline>();
    public DbSet<PipelineStage> PipelineStages => Set<PipelineStage>();

    // Product Catalog
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();

    // Entity Attachments & Collaboration
    public DbSet<EntityNote> EntityNotes => Set<EntityNote>();
    public DbSet<EntityComment> EntityComments => Set<EntityComment>();
    public DbSet<EntityImage> EntityImages => Set<EntityImage>();
    public DbSet<EntityFile> EntityFiles => Set<EntityFile>();
    public DbSet<EntityDocument> EntityDocuments => Set<EntityDocument>();
    public DbSet<EntityParticipant> EntityParticipants => Set<EntityParticipant>();
    public DbSet<EntityActivityParticipant> EntityActivityParticipants => Set<EntityActivityParticipant>();
    public DbSet<NoteReaction> NoteReactions => Set<NoteReaction>();
    public DbSet<EntityLabel> EntityLabels => Set<EntityLabel>();
    public DbSet<Label> Labels => Set<Label>();
    public DbSet<EntityPrice> EntityPrices => Set<EntityPrice>();

    // Automation & Workflows
    public DbSet<Sequence> Sequences => Set<Sequence>();
    public DbSet<SequenceStep> SequenceSteps => Set<SequenceStep>();
    public DbSet<EntitySequenceEnrollment> EntitySequenceEnrollments => Set<EntitySequenceEnrollment>();
    public DbSet<AssignmentRule> AssignmentRules => Set<AssignmentRule>();
    public DbSet<AssignmentRuleCondition> AssignmentRuleConditions => Set<AssignmentRuleCondition>();
    public DbSet<AssignmentRuleHistory> AssignmentRuleHistories => Set<AssignmentRuleHistory>();
    public DbSet<AssignmentRulesSet> AssignmentRulesSets => Set<AssignmentRulesSet>();

    // Scheduler
    public DbSet<Scheduler> Schedulers => Set<Scheduler>();
    public DbSet<SchedulerAvailability> SchedulerAvailabilities => Set<SchedulerAvailability>();
    public DbSet<SchedulerSlot> SchedulerSlots => Set<SchedulerSlot>();
    public DbSet<SchedulerBooking> SchedulerBookings => Set<SchedulerBooking>();

    // Scoring
    public DbSet<ScoringProfile> ScoringProfiles => Set<ScoringProfile>();
    public DbSet<ScoringCriteria> ScoringCriterias => Set<ScoringCriteria>();
    public DbSet<ScoringGroup> ScoringGroups => Set<ScoringGroup>();
    public DbSet<ScoringRuleCondition> ScoringRuleConditions => Set<ScoringRuleCondition>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Apply tenant and soft delete filters
        builder.ApplyTenantFilters(ContextManager);
        builder.ApplySoftDeleteFilters();
        builder.ApplySuspendibleFilters();
    }
}