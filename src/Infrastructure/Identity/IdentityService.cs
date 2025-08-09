using System.Security.Claims;
using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Application.Common.Models;
using ConnectFlow.Domain.Constants;
using ConnectFlow.Domain.Entities;
using ConnectFlow.Domain.Enums;
using ConnectFlow.Domain.Events.Mediator.Subscriptions;
using ConnectFlow.Domain.Events.Mediator.Tenants;
using ConnectFlow.Domain.Events.Mediator.TenantUsers;
using ConnectFlow.Domain.Events.Mediator.Users;
using ConnectFlow.Infrastructure.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ConnectFlow.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthTokenService _authTokenService;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;

    public IdentityService(
        IApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IMediator mediator,
        ICurrentUserService currentUserService,
        IAuthTokenService authTokenService,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService)
    {
        _context = context;
        _userManager = userManager;
        _mediator = mediator;
        _currentUserService = currentUserService;
        _authTokenService = authTokenService;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
    }

    #region User and Tenant Management

    public async Task<Result<UserToken>> CreateTenantForNewUserAsync(string email, string password, string firstName, string lastName, string? jobTitle, string? phoneNumber, string? mobile, string? timeZone, string? locale, SubscriptionPlan plan = SubscriptionPlan.Free)
    {
        var validRoles = new[] { Roles.TenantAdmin };

        var userCreationResult = await CreateApplicationUserAsync(email, password, firstName, lastName, jobTitle, phoneNumber, mobile, timeZone, locale, true, validRoles);

        if (!userCreationResult.Result.Succeeded || userCreationResult.Result.Data == null)
        {
            return Result<UserToken>.Failure(userCreationResult.Result.Errors, null);
        }

        var newUser = userCreationResult.Result.Data;

        var tenantResult = await CreateTenant($"{firstName}'s Tenant", newUser);

        if (!tenantResult.Succeeded || tenantResult.Data == null)
        {
            return Result<UserToken>.Failure(tenantResult.Errors, null);
        }

        var tenant = tenantResult.Data;

        // Join the tenant as the admin user
        await JoinTenantInternalAsync(newUser, tenant.Id, validRoles, _currentUserService.GetCurrentApplicationUserId());

        // Create subscription
        await CreateSubscriptionForTenantAsync(tenant, plan);

        return Result<UserToken>.Success(new UserToken() { ApplicationUserId = newUser.PublicId, Token = userCreationResult.ConfirmationToken });
    }

    public async Task<Result> CreateTenantForExistingUserAsync(string email, SubscriptionPlan plan = SubscriptionPlan.Free)
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
        await JoinTenantInternalAsync(existingUser, tenant.Id, validRoles, _currentUserService.GetCurrentApplicationUserId());

        // Add role to user's global roles if not already present
        foreach (var role in validRoles)
        {
            if (!await _userManager.IsInRoleAsync(existingUser, role))
            {
                await _userManager.AddToRoleAsync(existingUser, role);
            }
        }

        // Create subscription
        await CreateSubscriptionForTenantAsync(tenant, plan);

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
                await JoinTenantInternalAsync(result.Result.Data, tenantId.Value, validRoles, _currentUserService.GetCurrentApplicationUserId());
            }

            return Result<UserToken>.Success(new UserToken() { ApplicationUserId = result.Result.Data.PublicId, Token = result.ConfirmationToken });
        }

        return Result<UserToken>.Failure(result.Result.Errors, null);
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

        await JoinTenantInternalAsync(existingUser, tenantId, validRoles, _currentUserService.GetCurrentApplicationUserId());

        foreach (var role in validRoles)
        {
            if (!await _userManager.IsInRoleAsync(existingUser, role))
            {
                await _userManager.AddToRoleAsync(existingUser, role);
            }
        }

        return Result.Success();
    }

    public async Task<Result> ConfirmEmailAsync(Guid userId, string confirmationToken)
    {
        var user = await _userManager.FindByPublicIdAsync(userId);

        if (user == null || user.IsActive == false)
        {
            return Result.Failure(new[] { "user not found" });
        }

        if (await _userManager.IsEmailConfirmedAsync(user))
        {
            return Result.Failure(new[] { "already confirmed" });
        }

        var result = await _userManager.ConfirmEmailAsync(user, confirmationToken);

        if (result.Succeeded)
        {
            await _mediator.Publish(new UserEmailConfirmedEvent(user.Id, user.PublicId, user.Email!, user.FirstName, user.LastName, user.JobTitle, user.PhoneNumber, user.Mobile, user.TimeZone, user.Locale, user.EmailConfirmed, confirmationToken));
        }

        return result.ToApplicationResult();
    }

    public async Task<Result<UserToken>> ResetPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null || user.IsActive == false)
        {
            return Result<UserToken>.Failure(new[] { "user not found" }, null);
        }

        string resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        await _mediator.Publish(new UserPasswordResetEvent(user.Id, user.PublicId, user.Email!, user.FirstName, user.LastName, user.JobTitle, user.PhoneNumber, user.Mobile, user.TimeZone, user.Locale, user.EmailConfirmed, resetPasswordToken));

        return Result<UserToken>.Success(new UserToken() { ApplicationUserId = user.PublicId, Token = resetPasswordToken });
    }

    public async Task<Result> UpdatePasswordAsync(Guid userId, string passwordToken, string newPassword)
    {
        var user = await _userManager.FindByPublicIdAsync(userId);

        if (user == null || user.IsActive == false)
        {
            return Result.Failure(new[] { "user not found" });
        }

        var result = await _userManager.ResetPasswordAsync(user, passwordToken, newPassword);

        if (result.Succeeded)
        {
            await _mediator.Publish(new UserPasswordUpdateEvent(user.Id, user.PublicId, user.Email!, user.FirstName, user.LastName, user.JobTitle, user.PhoneNumber, user.Mobile, user.TimeZone, user.Locale, user.EmailConfirmed));
        }

        return result.ToApplicationResult();
    }

    public async Task<Result> ChangePasswordAsync(Guid userId, string password, string newPassword)
    {
        var user = await _userManager.FindByPublicIdAsync(userId);

        if (user == null || user.IsActive == false)
        {
            return Result.Failure(new[] { "user not found" });
        }

        var result = await _userManager.ChangePasswordAsync(user, password, newPassword);

        if (result.Succeeded)
        {
            await _mediator.Publish(new UserPasswordUpdateEvent(user.Id, user.PublicId, user.Email!, user.FirstName, user.LastName, user.JobTitle, user.PhoneNumber, user.Mobile, user.TimeZone, user.Locale, user.EmailConfirmed));
        }

        return result.ToApplicationResult();
    }

    public async Task<Result<AuthToken>> SignInAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null || user.IsActive == false)
        {
            return Result<AuthToken>.Failure(new[] { "user not found" }, null);
        }

        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            return Result<AuthToken>.Failure(new[] { "account not confirmed yet" }, null);
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password!);

        if (!isPasswordValid)
        {
            return Result<AuthToken>.Failure(new[] { "failed to authenticate" }, null);
        }

        // Generate access and refresh tokens
        return await ManageTokensAsync(user, true);
    }

    public async Task<Result<AuthToken>> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var principal = _authTokenService.GetPrincipalFromExpiredToken(accessToken);
        if (principal == null)
        {
            return Result<AuthToken>.Failure(new[] { "Invalid access token or refresh token" }, null);
        }

        string? email = principal.FindFirstValue(ClaimTypes.Email);

        var user = await _userManager.FindByEmailAsync(email!);

        if (user == null || user.RefreshToken != refreshToken || (user.RefreshTokenExpiryTime.HasValue && user.RefreshTokenExpiryTime.Value <= DateTimeOffset.UtcNow))
        {
            return Result<AuthToken>.Failure(new[] { "Invalid access token or refresh token" }, null);
        }

        //refresh token remains same for specified days
        return await ManageTokensAsync(user, false);
    }

    public async Task<string?> GetUserNameAsync(Guid userId)
    {
        var user = await _userManager.FindByPublicIdAsync(userId);

        return user?.UserName;
    }

    public async Task<bool> IsInRoleAsync(Guid userId, string role)
    {
        var user = await _userManager.FindByPublicIdAsync(userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<Result<(Guid UserId, string FirstName, string LastName, string Email)>> GetUserAsync(Guid userId)
    {
        var user = await _userManager.FindByPublicIdAsync(userId);

        if (user == null || user.Email == null)
        {
            return Result<(Guid, string, string, string)>.Failure(new[] { "user not found" }, default);
        }

        return Result<(Guid UserId, string FirstName, string LastName, string Email)>.Success((user.PublicId, user.FirstName, user.LastName, user.Email));
    }

    public async Task<bool> AuthorizeAsync(Guid userId, string policyName)
    {
        var user = await _userManager.FindByPublicIdAsync(userId);

        if (user == null || !user.IsActive || !user.EmailConfirmed)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> RevokeAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Result.Failure(new[] { "user not found" });
        }

        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);

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

    #endregion

    #region Private Methods

    private async Task<Result<Tenant>> CreateTenant(string name, ApplicationUser adminUser)
    {
        var tenant = new Tenant
        {
            Name = name,
            Settings = "{}",
            CreatedBy = _currentUserService.GetCurrentApplicationUserId() ?? adminUser.Id
        };

        tenant.AddDomainEvent(new TenantCreatedEvent(tenant));

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();

        return Result<Tenant>.Success(tenant);
    }

    private async Task<(Result<ApplicationUser> Result, string ConfirmationToken)> CreateApplicationUserAsync(string email, string password, string firstName, string lastName, string? jobTitle, string? phoneNumber, string? mobile, string? timeZone, string? locale, bool requireEmailConfirmation, string[] roles)
    {
        var user = new ApplicationUser
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

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            if (roles.Any())
            {
                // Add user to specified roles
                await _userManager.AddToRolesAsync(user, roles);
            }

            string confirmationToken = string.Empty;
            if (requireEmailConfirmation)
            {
                confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            }

            await _mediator.Publish(new UserCreatedEvent(user.Id, user.PublicId, user.Email, user.FirstName, user.LastName, user.JobTitle, user.PhoneNumber, user.Mobile, user.TimeZone, user.Locale, user.EmailConfirmed, confirmationToken));

            return (Result<ApplicationUser>.Success(user), confirmationToken);
        }

        return (result.ToApplicationResult(user), string.Empty);
    }

    private async Task JoinTenantInternalAsync(ApplicationUser user, int tenantId, string[] roles, int? invitedBy)
    {
        var tenantUser = new TenantUser
        {
            UserId = user.Id,
            TenantId = tenantId,
            InvitedBy = invitedBy,
            Status = TenantUserStatus.Active,
            CreatedBy = invitedBy ?? user.Id,
            JoinedAt = DateTimeOffset.UtcNow,
            IsActive = true
        };

        tenantUser.AddDomainEvent(new TenantUserJoinedEvent(tenantUser));

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
                AssignedBy = invitedBy ?? user.Id,
                CreatedBy = invitedBy ?? user.Id
            };

            _context.TenantUserRoles.Add(tenantUserRole);
        }

        await _context.SaveChangesAsync();
    }

    private async Task<Result<Subscription>> CreateSubscriptionForTenantAsync(Tenant tenant, SubscriptionPlan plan)
    {
        var subscription = SubscriptionPlans.GetPlanByName(plan);
        subscription.TenantId = tenant.Id;
        subscription.CreatedBy = _currentUserService.GetCurrentApplicationUserId();

        subscription.AddDomainEvent(new SubscriptionCreatedEvent(subscription));

        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();

        return Result<Subscription>.Success(subscription);
    }

    // private async Task<Result> IsValidTenantAsync(string code, string domain)
    // {
    //     // Check if tenant code or domain already exists
    //     var existingTenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Code == code || t.Domain == domain);

    //     if (existingTenant != null)
    //     {
    //         return Result.Failure(new[] { existingTenant.Code == code ? "Tenant with this code already exists" : "Tenant with this domain already exists" });
    //     }

    //     return Result.Success();
    // }

    private async Task<Result<AuthToken>> ManageTokensAsync(ApplicationUser user, bool generateNewRefreshToken)
    {
        var accessTokenResult = await _authTokenService.CreateAccessTokenAsync(user);
        var refreshToken = user.RefreshToken ?? string.Empty;

        if (generateNewRefreshToken)
        {
            var refreshTokenResult = _authTokenService.CreateRefreshToken();

            refreshToken = refreshTokenResult.RefreshToken;

            user.RefreshToken = refreshTokenResult.RefreshToken;
            user.RefreshTokenExpiryTime = refreshTokenResult.Expiry;
        }

        user.LastLoginAt = DateTimeOffset.UtcNow;
        await _userManager.UpdateAsync(user);

        AuthToken authToken = new() { ApplicationUserId = user.PublicId, Email = user.Email!, ExpiresIn = accessTokenResult.ExpiresInMinutes * 60, AccessToken = accessTokenResult.AccessToken, RefreshToken = refreshToken };

        return Result<AuthToken>.Success(authToken);
    }

    #endregion
}