namespace ConnectFlow.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<TenantUser> TenantUsers { get; }
    DbSet<TenantUserRole> TenantUserRoles { get; }
    DbSet<Subscription> Subscriptions { get; }
    DbSet<Plan> Plans { get; }
    DbSet<ChannelAccount> ChannelAccounts { get; }
    DbSet<EntityActivity> Activities { get; }
    DbSet<EntityChangeLog> ChangeLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}