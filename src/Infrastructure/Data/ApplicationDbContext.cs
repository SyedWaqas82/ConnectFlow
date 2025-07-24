using System.Reflection;
using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Domain.Entities;
using ConnectFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ConnectFlow.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<TodoList> TodoLists => Set<TodoList>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    public DbSet<AIUsage> AIUsages => Set<AIUsage>();

    public DbSet<Appointment> Appointments => Set<Appointment>();

    public DbSet<Attachment> Attachments => Set<Attachment>();

    public DbSet<Channel> Channels => Set<Channel>();

    public DbSet<ChannelAccount> ChannelAccounts => Set<ChannelAccount>();

    public DbSet<ChannelUser> ChannelUsers => Set<ChannelUser>();

    public DbSet<Company> Companies => Set<Company>();

    public DbSet<Contact> Contacts => Set<Contact>();

    public DbSet<ContactScore> ContactScores => Set<ContactScore>();

    public DbSet<ContentCategory> ContentCategories => Set<ContentCategory>();

    public DbSet<Conversation> Conversations => Set<Conversation>();

    public DbSet<ConversationParticipant> ConversationParticipants => Set<ConversationParticipant>();

    public DbSet<CustomField> CustomFields => Set<CustomField>();

    public DbSet<Lead> Leads => Set<Lead>();

    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();

    public DbSet<Message> Messages => Set<Message>();

    public DbSet<Pipeline> Pipelines => Set<Pipeline>();

    public DbSet<Reminder> Reminders => Set<Reminder>();

    public DbSet<ReminderRecipient> ReminderRecipients => Set<ReminderRecipient>();

    public DbSet<Resource> Resources => Set<Resource>();

    public DbSet<SearchIndex> SearchIndexes => Set<SearchIndex>();

    public DbSet<Service> Services => Set<Service>();

    public DbSet<Stage> Stages => Set<Stage>();

    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    public DbSet<Tag> Tags => Set<Tag>();

    public DbSet<Team> Teams => Set<Team>();

    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();

    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<TenantUser> TenantUsers => Set<TenantUser>();

    public DbSet<TenantUserRole> TenantUserRoles => Set<TenantUserRole>();

    public DbSet<Template> Templates => Set<Template>();

    public DbSet<Trigger> Triggers => Set<Trigger>();

    public DbSet<Workflow> Workflows => Set<Workflow>();

    public DbSet<WorkTask> WorkTasks => Set<WorkTask>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
