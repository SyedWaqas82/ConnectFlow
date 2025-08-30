using System.Security.Claims;
using ConnectFlow.Application.Common.Models;
using ConnectFlow.Domain.Constants;
using ConnectFlow.Domain.Events.Mediator.Tenants;
using ConnectFlow.Domain.Events.Mediator.TenantUsers;
using ConnectFlow.Domain.Events.Mediator.Users;
using ConnectFlow.Infrastructure.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ConnectFlow.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMediator _mediator;
    private readonly IContextManager _contextManager;
    private readonly IAuthTokenService _authTokenService;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly IPaymentService _paymentService;

    public IdentityService(
        IApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IMediator mediator,
        IContextManager contextManager,
        IAuthTokenService authTokenService,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        IPaymentService paymentService)
    {
        _context = context;
        _userManager = userManager;
        _mediator = mediator;
        _contextManager = contextManager;
        _authTokenService = authTokenService;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _paymentService = paymentService;
    }

    #region User and Tenant Management

    public async Task<Result<UserToken>> CreateTenantForNewUserAsync(string email, string password, string firstName, string lastName, string? jobTitle, string? phoneNumber, string? mobile, string? timeZone, string? locale)
    {
        var validRoles = new[] { Roles.TenantAdmin };

        var userCreationResult = await CreateApplicationUserAsync(email, password, firstName, lastName, jobTitle, phoneNumber, mobile, timeZone, locale, true, validRoles);

        if (!userCreationResult.Result.Succeeded || userCreationResult.Result.Data == null)
        {
            return Result<UserToken>.Failure(null, userCreationResult.Result.Errors);
        }

        var newUser = userCreationResult.Result.Data;

        var tenantResult = await CreateTenant($"{firstName}'s Tenant", newUser);

        if (!tenantResult.Succeeded || tenantResult.Data == null)
        {
            return Result<UserToken>.Failure(null, tenantResult.Errors);
        }

        var tenant = tenantResult.Data;

        // Join the tenant as the admin user
        await JoinTenantInternalAsync(newUser, tenant.Id, validRoles, _contextManager.GetCurrentApplicationUserId());

        return Result<UserToken>.Success(new UserToken() { ApplicationUserPublicId = newUser.PublicId, Token = userCreationResult.ConfirmationToken });
    }

    public async Task<Result> CreateTenantForExistingUserAsync(string email)
    {
        var validRoles = new[] { Roles.TenantAdmin };

        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser == null || existingUser.IsActive == false)
        {
            return Result.Failure(new[] { "user not found" });
        }

        var tenantResult = await CreateTenant($"{existingUser.FirstName}'s Tenant", existingUser);

        if (!tenantResult.Succeeded || tenantResult.Data == null)
        {
            return Result.Failure(tenantResult.Errors);
        }

        var tenant = tenantResult.Data;

        // Join the tenant as the admin user
        await JoinTenantInternalAsync(existingUser, tenant.Id, validRoles, _contextManager.GetCurrentApplicationUserId());

        // Add role to user's global roles if not already present
        foreach (var role in validRoles)
        {
            if (!await _userManager.IsInRoleAsync(existingUser, role))
            {
                await _userManager.AddToRoleAsync(existingUser, role);
            }
        }

        return Result.Success();
    }

    public async Task<Result<UserToken>> JoinTenantAsNewUserAsync(string email, string password, string firstName, string lastName, string[] roles, string? jobTitle, string? phoneNumber, string? mobile, string? timeZone, string? locale, int? tenantId)
    {
        var validRoles = roles.Where(r => Roles.AllRoles.Contains(r)).ToArray();
        validRoles = validRoles.Length > 0 ? validRoles : new[] { Roles.NonTenantAdmin };

        var result = await CreateApplicationUserAsync(email, password, firstName, lastName, jobTitle, phoneNumber, mobile, timeZone, locale, true, validRoles);

        if (result.Result.Succeeded && result.Result.Data != null)
        {
            if (tenantId.HasValue)
            {
                await JoinTenantInternalAsync(result.Result.Data, tenantId.Value, validRoles, _contextManager.GetCurrentApplicationUserId());
            }

            return Result<UserToken>.Success(new UserToken() { ApplicationUserPublicId = result.Result.Data.PublicId, Token = result.ConfirmationToken });
        }

        return Result<UserToken>.Failure(null, result.Result.Errors);
    }

    public async Task<Result> JoinTenantAsExistingUserAsync(string email, string[] roles, int tenantId)
    {
        var validRoles = roles.Where(r => Roles.AllRoles.Contains(r)).ToArray();
        validRoles = validRoles.Length > 0 ? validRoles : new[] { Roles.NonTenantAdmin };

        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser == null || existingUser.IsActive == false)
        {
            return Result.Failure(new[] { "user not found" });
        }

        await JoinTenantInternalAsync(existingUser, tenantId, validRoles, _contextManager.GetCurrentApplicationUserId());

        foreach (var role in validRoles)
        {
            if (!await _userManager.IsInRoleAsync(existingUser, role))
            {
                await _userManager.AddToRoleAsync(existingUser, role);
            }
        }

        return Result.Success();
    }

    public async Task<Result> ConfirmEmailAsync(Guid applicationUserPublicId, string confirmationToken)
    {
        var appUser = await _userManager.FindByPublicIdAsync(applicationUserPublicId);

        if (appUser == null || appUser.IsActive == false)
        {
            return Result.Failure(new[] { "user not found" });
        }

        if (await _userManager.IsEmailConfirmedAsync(appUser))
        {
            return Result.Failure(new[] { "already confirmed" });
        }

        var result = await _userManager.ConfirmEmailAsync(appUser, confirmationToken);

        if (result.Succeeded)
        {
            await _mediator.Publish(new UserEmailConfirmedEvent(default, appUser.Id)
            {
                ApplicationUserPublicId = appUser.PublicId,
                CorrelationId = _contextManager.GetCorrelationId(),
                Email = appUser.Email!,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
            });
        }

        return result.ToApplicationResult();
    }

    public async Task<Result<UserToken>> ResetPasswordAsync(string email)
    {
        var appUser = await _userManager.FindByEmailAsync(email);

        if (appUser == null || appUser.IsActive == false)
        {
            return Result<UserToken>.Failure(null, new[] { "user not found" });
        }

        string resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(appUser);

        await _mediator.Publish(new UserPasswordResetEvent(default, appUser.Id)
        {
            ApplicationUserPublicId = appUser.PublicId,
            CorrelationId = _contextManager.GetCorrelationId(),
            Email = appUser.Email!,
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            ResetPasswordToken = resetPasswordToken
        });

        return Result<UserToken>.Success(new UserToken() { ApplicationUserPublicId = appUser.PublicId, Token = resetPasswordToken });
    }

    public async Task<Result> UpdatePasswordAsync(Guid applicationUserPublicId, string passwordToken, string newPassword)
    {
        var appUser = await _userManager.FindByPublicIdAsync(applicationUserPublicId);

        if (appUser == null || appUser.IsActive == false)
        {
            return Result.Failure(new[] { "user not found" });
        }

        var result = await _userManager.ResetPasswordAsync(appUser, passwordToken, newPassword);

        if (result.Succeeded)
        {
            await _mediator.Publish(new UserPasswordUpdateEvent(default, appUser.Id)
            {
                ApplicationUserPublicId = appUser.PublicId,
                CorrelationId = _contextManager.GetCorrelationId(),
                Email = appUser.Email!,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
            });
        }

        return result.ToApplicationResult();
    }

    public async Task<Result> ChangePasswordAsync(Guid applicationUserPublicId, string password, string newPassword)
    {
        var appUser = await _userManager.FindByPublicIdAsync(applicationUserPublicId);

        if (appUser == null || appUser.IsActive == false)
        {
            return Result.Failure(new[] { "user not found" });
        }

        var result = await _userManager.ChangePasswordAsync(appUser, password, newPassword);

        if (result.Succeeded)
        {
            await _mediator.Publish(new UserPasswordUpdateEvent(default, appUser.Id)
            {
                ApplicationUserPublicId = appUser.PublicId,
                CorrelationId = _contextManager.GetCorrelationId(),
                Email = appUser.Email!,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
            });
        }

        return result.ToApplicationResult();
    }

    public async Task<Result<AuthToken>> SignInAsync(string email, string password)
    {
        var appUser = await _userManager.FindByEmailAsync(email);

        if (appUser == null || appUser.IsActive == false)
        {
            return Result<AuthToken>.Failure(null, new[] { "user not found" });
        }

        // if (!await _userManager.IsEmailConfirmedAsync(appUser))
        // {
        //     return Result<AuthToken>.Failure(new[] { "account not confirmed yet" }, null);
        // }

        var isPasswordValid = await _userManager.CheckPasswordAsync(appUser, password!);

        if (!isPasswordValid)
        {
            return Result<AuthToken>.Failure(null, new[] { "failed to authenticate" });
        }

        // Generate access and refresh tokens
        return await ManageTokensAsync(appUser, true);
    }

    public async Task<Result<AuthToken>> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var principal = _authTokenService.GetPrincipalFromExpiredToken(accessToken);
        if (principal == null)
        {
            return Result<AuthToken>.Failure(null, new[] { "Invalid access token or refresh token" });
        }

        string? email = principal.FindFirstValue(ClaimTypes.Email);

        var appUser = await _userManager.FindByEmailAsync(email!);

        if (appUser == null || appUser.RefreshToken != refreshToken || (appUser.RefreshTokenExpiryTime.HasValue && appUser.RefreshTokenExpiryTime.Value <= DateTimeOffset.UtcNow))
        {
            return Result<AuthToken>.Failure(null, new[] { "Invalid access token or refresh token" });
        }

        //refresh token remains same for specified days
        return await ManageTokensAsync(appUser, false);
    }

    public async Task<string?> GetUserNameAsync(Guid applicationUserPublicId)
    {
        var appUser = await _userManager.FindByPublicIdAsync(applicationUserPublicId);

        return appUser?.UserName;
    }

    public async Task<bool> IsInRoleAsync(Guid applicationUserPublicId, string role)
    {
        var appUser = await _userManager.FindByPublicIdAsync(applicationUserPublicId);

        return appUser != null && await _userManager.IsInRoleAsync(appUser, role);
    }

    public async Task<Result<(Guid ApplicationUserPublicId, string FirstName, string LastName, string Email)>> GetUserAsync(Guid applicationUserPublicId)
    {
        var appUser = await _userManager.FindByPublicIdAsync(applicationUserPublicId);

        if (appUser == null || appUser.Email == null)
        {
            return Result<(Guid, string, string, string)>.Failure(default, new[] { "user not found" });
        }

        return Result<(Guid ApplicationUserPublicId, string FirstName, string LastName, string Email)>.Success((appUser.PublicId, appUser.FirstName, appUser.LastName, appUser.Email));
    }

    public async Task<Result<(Guid ApplicationUserPublicId, string FirstName, string LastName, string Email)>> GetUserAsync(int applicationUserId)
    {
        var appUser = await _userManager.FindByIdAsync(applicationUserId.ToString());

        if (appUser == null || appUser.Email == null)
        {
            return Result<(Guid, string, string, string)>.Failure(default, new[] { "user not found" });
        }

        return Result<(Guid ApplicationUserPublicId, string FirstName, string LastName, string Email)>.Success((appUser.PublicId, appUser.FirstName, appUser.LastName, appUser.Email));
    }

    public async Task<bool> AuthorizeAsync(Guid applicationUserPublicId, string policyName)
    {
        var appUser = await _userManager.FindByPublicIdAsync(applicationUserPublicId);

        //if (appUser == null || !appUser.IsActive || !appUser.EmailConfirmed)
        if (appUser == null || !appUser.IsActive)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(appUser);
        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> RevokeAsync(string email)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null)
        {
            return Result.Failure(new[] { "user not found" });
        }

        appUser.RefreshToken = null;
        await _userManager.UpdateAsync(appUser);

        return Result.Success();
    }

    public async Task<Result> RevokeAllAsync()
    {
        var users = _userManager.Users.ToList();
        foreach (var user in users)
        {
            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
        }

        return Result.Success();
    }

    /// <summary>
    /// Creates a Stripe customer for an existing tenant that doesn't have one.
    /// This is useful for migrating existing tenants to Stripe integration.
    /// </summary>
    public async Task<Result> CreateStripeCustomerForExistingTenantAsync(int tenantId)
    {
        try
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId);
            if (tenant == null)
            {
                return Result.Failure(new[] { "Tenant not found" });
            }

            if (!string.IsNullOrEmpty(tenant.PaymentProviderCustomerId))
            {
                return Result.Failure(new[] { "Tenant already has a Stripe customer" });
            }

            // Get tenant admin user for customer creation
            var tenantAdminId = await _context.TenantUsers
                .Where(tu => tu.TenantId == tenantId && tu.Status == TenantUserStatus.Active)
                .Join(_context.TenantUserRoles, tu => tu.Id, tur => tur.TenantUserId, (tu, tur) => new { tu, tur })
                .Where(x => x.tur.RoleName == Roles.TenantAdmin)
                .Select(x => x.tu.ApplicationUserId)
                .FirstOrDefaultAsync();

            if (tenantAdminId == 0)
            {
                return Result.Failure(new[] { "No active tenant admin found" });
            }

            var tenantAdmin = await _userManager.FindByIdAsync(tenantAdminId.ToString());
            if (tenantAdmin == null)
            {
                return Result.Failure(new[] { "Tenant admin user not found" });
            }

            // Create Stripe customer
            var stripeCustomer = await _paymentService.CreateCustomerAsync(
                tenant.Email,
                tenant.Name,
                new Dictionary<string, string>
                {
                    { "tenant_id", tenant.Id.ToString() },
                    { "tenant_name", tenant.Name },
                    { "admin_user_id", tenantAdmin.Id.ToString() },
                    { "admin_user_public_id", tenantAdmin.PublicId.ToString() },
                    { "migration", "true" }
                });

            // Update tenant with Stripe customer ID
            tenant.PaymentProviderCustomerId = stripeCustomer.Id;
            await _context.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new[] { $"Failed to create Stripe customer for tenant: {ex.Message}" });
        }
    }

    #endregion

    #region Private Methods

    private async Task<Result<Tenant>> CreateTenant(string name, ApplicationUser adminUser)
    {
        try
        {
            var tenant = new Tenant
            {
                Name = name,
                Email = adminUser.Email!,
                Settings = "{}",
                CreatedBy = _contextManager.GetCurrentApplicationUserId() ?? adminUser.Id
            };

            tenant.AddDomainEvent(new TenantCreatedEvent(tenant, adminUser.Id));

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            // Create Stripe customer first
            var stripeCustomer = await _paymentService.CreateCustomerAsync(
                adminUser.Email!,
                $"{adminUser.FirstName} {adminUser.LastName}",
                new Dictionary<string, string>
                {
                    { "tenant_id", tenant.Id.ToString() },
                    { "tenant_name", name },
                    { "admin_user_id", adminUser.Id.ToString() },
                    { "admin_user_public_id", adminUser.PublicId.ToString() }
                });

            tenant.PaymentProviderCustomerId = stripeCustomer.Id;
            await _context.SaveChangesAsync();

            return Result<Tenant>.Success(tenant);
        }
        catch (Exception ex)
        {
            // Log the error and return failure
            return Result<Tenant>.Failure(null, new[] { $"Failed to create tenant with Stripe customer: {ex.Message}" });
        }
    }

    private async Task<(Result<ApplicationUser> Result, string ConfirmationToken)> CreateApplicationUserAsync(string email, string password, string firstName, string lastName, string? jobTitle, string? phoneNumber, string? mobile, string? timeZone, string? locale, bool requireEmailConfirmation, string[] roles)
    {
        var appUser = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            JobTitle = jobTitle,
            PhoneNumber = phoneNumber,
            Mobile = mobile,
            TimeZone = timeZone ?? "UTC",
            Locale = locale ?? "en-US",
            EmailConfirmed = !requireEmailConfirmation,
            IsActive = true,
            Preferences = "{}" // Default preferences, can be updated later,
        };

        var result = await _userManager.CreateAsync(appUser, password);

        if (result.Succeeded)
        {
            if (roles.Any())
            {
                // Add appUser to specified roles
                await _userManager.AddToRolesAsync(appUser, roles);
            }

            string confirmationToken = string.Empty;
            if (requireEmailConfirmation)
            {
                confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
            }

            await _mediator.Publish(new UserCreatedEvent(default, appUser.Id)
            {
                ApplicationUserPublicId = appUser.PublicId,
                CorrelationId = _contextManager.GetCorrelationId(),
                Email = appUser.Email,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                JobTitle = appUser.JobTitle,
                PhoneNumber = appUser.PhoneNumber,
                Mobile = appUser.Mobile,
                TimeZone = appUser.TimeZone,
                Locale = appUser.Locale,
                EmailConfirmed = appUser.EmailConfirmed,
                ConfirmationToken = confirmationToken
            });

            return (Result<ApplicationUser>.Success(appUser), confirmationToken);
        }

        return (result.ToApplicationResult(appUser), string.Empty);
    }

    private async Task JoinTenantInternalAsync(ApplicationUser appUser, int tenantId, string[] roles, int? invitedBy)
    {
        var tenantUser = new TenantUser
        {
            ApplicationUserId = appUser.Id,
            TenantId = tenantId,
            InvitedBy = invitedBy,
            EntityStatus = EntityStatus.Active,
            CreatedBy = invitedBy ?? appUser.Id,
            JoinedAt = DateTimeOffset.UtcNow,
            Status = TenantUserStatus.Active
        };

        tenantUser.AddDomainEvent(new TenantUserJoinedEvent(tenantUser, appUser.Id));

        _context.TenantUsers.Add(tenantUser);
        await _context.SaveChangesAsync();

        // Add roles
        foreach (var role in roles)
        {
            var tenantUserRole = new TenantUserRole
            {
                TenantUserId = tenantUser.Id,
                RoleName = role,
                AssignedAt = DateTimeOffset.UtcNow,
                AssignedBy = invitedBy ?? appUser.Id,
                CreatedBy = invitedBy ?? appUser.Id
            };

            _context.TenantUserRoles.Add(tenantUserRole);
        }

        await _context.SaveChangesAsync();
    }

    private async Task<Result<AuthToken>> ManageTokensAsync(ApplicationUser appUser, bool generateNewRefreshToken)
    {
        var accessTokenResult = await _authTokenService.CreateAccessTokenAsync(appUser);
        var refreshToken = appUser.RefreshToken ?? string.Empty;

        if (generateNewRefreshToken)
        {
            var refreshTokenResult = _authTokenService.CreateRefreshToken();

            refreshToken = refreshTokenResult.RefreshToken;

            appUser.RefreshToken = refreshTokenResult.RefreshToken;
            appUser.RefreshTokenExpiryTime = refreshTokenResult.Expiry;
        }

        appUser.LastLoginAt = DateTimeOffset.UtcNow;
        await _userManager.UpdateAsync(appUser);

        AuthToken authToken = new() { ApplicationUserPublicId = appUser.PublicId, Email = appUser.Email!, ExpiresIn = accessTokenResult.ExpiresInMinutes * 60, AccessToken = accessTokenResult.AccessToken, RefreshToken = refreshToken };

        return Result<AuthToken>.Success(authToken);
    }

    #endregion
}