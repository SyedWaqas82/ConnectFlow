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

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantUser> TenantUsers => Set<TenantUser>();
    public DbSet<TenantUserRole> TenantUserRoles => Set<TenantUserRole>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<ChannelAccount> ChannelAccounts => Set<ChannelAccount>();
    public DbSet<EntityActivity> Activities => Set<EntityActivity>();
    public DbSet<EntityChangeLog> ChangeLogs => Set<EntityChangeLog>();

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