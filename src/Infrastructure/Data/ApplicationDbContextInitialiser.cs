using ConnectFlow.Domain.Constants;
using ConnectFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;

namespace ConnectFlow.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static void AddAsyncSeeding(this DbContextOptionsBuilder builder, IServiceProvider serviceProvider)
    {
        // Synchronous seeding delegate.
        // This is called when EF Core performs a synchronous store management operation (e.g., migration).
        // It must be fast and use only static or blocking code. Here, we call the async seed method synchronously.
        builder.UseSeeding((context, _) =>
        {
            var initialiser = serviceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
            initialiser.SeedAsync().GetAwaiter().GetResult();
        });

        // Asynchronous seeding delegate.
        // This is called when EF Core performs an asynchronous store management operation.
        // It allows for non-blocking, async code. Here, we await the async seed method.
        builder.UseAsyncSeeding(async (context, _, ct) =>
        {
            var initialiser = serviceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
            await initialiser.SeedAsync();
        });
    }

    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // 1. Ensure roles exist
        var superAdminRole = Roles.SuperAdmin;
        var tenantAdminRole = Roles.TenantAdmin;

        if (!await _roleManager.RoleExistsAsync(superAdminRole))
            await _roleManager.CreateAsync(new ApplicationRole(superAdminRole));
        if (!await _roleManager.RoleExistsAsync(tenantAdminRole))
            await _roleManager.CreateAsync(new ApplicationRole(tenantAdminRole));

        // 2. Create SuperAdmin user
        var superAdminEmail = "superadmin@connectflow.local";
        if (_userManager.Users.All(u => u.UserName != superAdminEmail))
        {
            var superAdmin = new ApplicationUser
            {
                UserName = superAdminEmail,
                Email = superAdminEmail,
                EmailConfirmed = true
            };
            await _userManager.CreateAsync(superAdmin, "SuperAdmin123!"); // Use a strong password in production
            await _userManager.AddToRoleAsync(superAdmin, superAdminRole);
        }

        // 3. Create Tenant, Subscription, and TenantAdmin user

        var tenantAdminEmail = "tenantadmin@connectflow.local";

        if (_userManager.Users.All(u => u.UserName != tenantAdminEmail))
        {
            var tenantAdmin = new ApplicationUser
            {
                UserName = tenantAdminEmail,
                Email = tenantAdminEmail,
                EmailConfirmed = true,
            };
            await _userManager.CreateAsync(tenantAdmin, "TenantAdmin123!"); // Use a strong password in production
            await _userManager.AddToRoleAsync(tenantAdmin, tenantAdminRole);

            var tenantName = "Default Tenant";
            var tenant = _context.Tenants.FirstOrDefault(t => t.Name == tenantName);
            if (tenant == null)
            {
                tenant = new Tenant
                {
                    Email = tenantAdmin.Email!,
                    Domain = "Default Tenant",
                    Name = "Default Tenant",
                    Description = "Default Tenant",
                    PaymentProviderCustomerId = "cus_default", // Placeholder, replace with actual Stripe customer ID
                    CreatedBy = tenantAdmin.Id,
                };

                _context.Tenants.Add(tenant);
                await _context.SaveChangesAsync();
            }

            var tenantUser = new TenantUser
            {
                ApplicationUserId = tenantAdmin.Id,
                TenantId = tenant.Id,
                InvitedBy = tenantAdmin.Id,
                CreatedBy = tenantAdmin.Id,
                JoinedAt = DateTimeOffset.UtcNow,
            };

            tenantUser.TenantUserRoles.Add(new TenantUserRole
            {
                RoleName = Roles.TenantAdmin,
                AssignedAt = DateTimeOffset.UtcNow,
                AssignedBy = tenantAdmin.Id,
                CreatedBy = tenantAdmin.Id
            });

            _context.TenantUsers.Add(tenantUser);
            await _context.SaveChangesAsync();

            var subscription = _context.Subscriptions.FirstOrDefault(s => s.TenantId == tenant.Id);
            if (subscription == null)
            {
                subscription = new Subscription
                {
                    PaymentProviderSubscriptionId = "sub_default", // Placeholder, replace with actual Stripe subscription
                    Status = SubscriptionStatus.Active,
                    CurrentPeriodStart = DateTimeOffset.UtcNow,
                    PlanId = 2,
                    Amount = 29.99m,
                    Currency = "usd",
                    TenantId = tenant.Id,
                    CreatedBy = tenantAdmin.Id
                };

                _context.Subscriptions.Add(subscription);
                await _context.SaveChangesAsync();
            }
        }
    }
}