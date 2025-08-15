using ConnectFlow.Domain.Entities;

namespace ConnectFlow.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<TenantUser> TenantUsers { get; }
    DbSet<TenantUserRole> TenantUserRoles { get; }
    DbSet<Subscription> Subscriptions { get; }
    // DbSet<Lead> Leads { get; }
    // DbSet<Contact> Contacts { get; }
    // DbSet<Company> Companies { get; }
    // DbSet<CustomField> CustomFields { get; }
    // DbSet<AIUsage> AIUsages { get; }
    // DbSet<Stage> Stages { get; }
    // DbSet<Pipeline> Pipelines { get; }
    // DbSet<ContactScore> ContactScores { get; }

    // DbSet<Appointment> Appointments { get; }
    // DbSet<Attachment> Attachments { get; }
    // DbSet<Channel> Channels { get; }
    // DbSet<ChannelAccount> ChannelAccounts { get; }
    // DbSet<ChannelUser> ChannelUsers { get; }


    // DbSet<ContentCategory> ContentCategories { get; }
    // DbSet<Conversation> Conversations { get; }
    // DbSet<ConversationParticipant> ConversationParticipants { get; }


    // DbSet<MediaAsset> MediaAssets { get; }
    // DbSet<Message> Messages { get; }

    // DbSet<Reminder> Reminders { get; }
    // DbSet<ReminderRecipient> ReminderRecipients { get; }
    // DbSet<Resource> Resources { get; }
    // DbSet<SearchIndex> SearchIndexes { get; }
    // DbSet<Service> Services { get; }


    // DbSet<Tag> Tags { get; }
    // DbSet<Team> Teams { get; }
    // DbSet<TeamMember> TeamMembers { get; }
    // DbSet<Template> Templates { get; }
    // DbSet<Trigger> Triggers { get; }
    // DbSet<Workflow> Workflows { get; }
    // DbSet<WorkTask> WorkTasks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}