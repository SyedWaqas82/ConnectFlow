namespace ConnectFlow.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    // Tenant Management
    DbSet<Tenant> Tenants { get; }
    DbSet<TenantUser> TenantUsers { get; }
    DbSet<TenantUserRole> TenantUserRoles { get; }
    DbSet<Subscription> Subscriptions { get; }
    DbSet<Plan> Plans { get; }
    DbSet<ChannelAccount> ChannelAccounts { get; }

    // Activity Tracking
    DbSet<EntityActivity> Activities { get; }
    DbSet<EntityChangeLog> ChangeLogs { get; }

    // CRM - Deals
    DbSet<Deal> Deals { get; }
    DbSet<DealProduct> DealProducts { get; }
    DbSet<DealStageHistory> DealStageHistories { get; }
    DbSet<DealInstallment> DealInstallments { get; }

    // CRM - Contacts
    DbSet<Person> People { get; }
    DbSet<PersonEmail> PersonEmails { get; }
    DbSet<PersonPhone> PersonPhones { get; }
    DbSet<Organization> Organizations { get; }
    DbSet<OrganizationRelationship> OrganizationRelationships { get; }
    DbSet<Lead> Leads { get; }

    // Project Management
    DbSet<Project> Projects { get; }
    DbSet<ProjectBoard> ProjectBoards { get; }
    DbSet<ProjectPhase> ProjectPhases { get; }
    DbSet<ProjectTask> ProjectTasks { get; }
    DbSet<ProjectDeal> ProjectDeals { get; }

    // Sales Pipeline
    DbSet<Pipeline> Pipelines { get; }
    DbSet<PipelineStage> PipelineStages { get; }

    // Product Catalog
    DbSet<Product> Products { get; }
    DbSet<ProductVariant> ProductVariants { get; }
    DbSet<ProductCategory> ProductCategories { get; }

    // Entity Attachments & Collaboration
    DbSet<EntityNote> EntityNotes { get; }
    DbSet<EntityComment> EntityComments { get; }
    DbSet<EntityImage> EntityImages { get; }
    DbSet<EntityFile> EntityFiles { get; }
    DbSet<EntityDocument> EntityDocuments { get; }
    DbSet<EntityParticipant> EntityParticipants { get; }
    DbSet<EntityActivityParticipant> EntityActivityParticipants { get; }
    DbSet<NoteReaction> NoteReactions { get; }
    DbSet<EntityLabel> EntityLabels { get; }
    DbSet<Label> Labels { get; }
    DbSet<EntityPrice> EntityPrices { get; }

    // Automation & Workflows
    DbSet<Sequence> Sequences { get; }
    DbSet<SequenceStep> SequenceSteps { get; }
    DbSet<EntitySequenceEnrollment> EntitySequenceEnrollments { get; }
    DbSet<AssignmentRule> AssignmentRules { get; }
    DbSet<AssignmentRuleCondition> AssignmentRuleConditions { get; }
    DbSet<AssignmentRuleHistory> AssignmentRuleHistories { get; }
    DbSet<AssignmentRulesSet> AssignmentRulesSets { get; }

    // Scheduler
    DbSet<Scheduler> Schedulers { get; }
    DbSet<SchedulerAvailability> SchedulerAvailabilities { get; }
    DbSet<SchedulerSlot> SchedulerSlots { get; }
    DbSet<SchedulerBooking> SchedulerBookings { get; }

    // Scoring
    DbSet<ScoringProfile> ScoringProfiles { get; }
    DbSet<ScoringCriteria> ScoringCriterias { get; }
    DbSet<ScoringGroup> ScoringGroups { get; }
    DbSet<ScoringRuleCondition> ScoringRuleConditions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}