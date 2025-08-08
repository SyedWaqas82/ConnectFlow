using ConnectFlow.Application.Common.Models;
using ConnectFlow.Domain.Enums;

namespace ConnectFlow.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<UserToken>> CreateTenantForNewUserAsync(string email, string password, string firstName, string lastName, string? jobTitle = null, string? phoneNumber = null, string? mobile = null, string? timeZone = null, string? locale = null, SubscriptionPlan plan = SubscriptionPlan.Free);
    Task<Result> CreateTenantForExistingUserAsync(string email, SubscriptionPlan plan = SubscriptionPlan.Free);
    Task<Result<UserToken>> JoinTenantAsNewUserAsync(string email, string password, string firstName, string lastName, string[] roles, string? jobTitle = null, string? phoneNumber = null, string? mobile = null, string? timeZone = null, string? locale = null, int? tenantId = null);
    Task<Result> JoinTenantAsExistingUserAsync(string email, string[] roles, int tenantId);
    Task<Result> ConfirmEmailAsync(Guid userId, string confirmationToken);
    Task<Result<UserToken>> ResetPasswordAsync(string email);
    Task<Result> UpdatePasswordAsync(Guid userId, string passwordToken, string newPassword);
    Task<Result> ChangePasswordAsync(Guid userId, string password, string newPassword);
    Task<Result<AuthToken>> SignInAsync(string email, string password);
    Task<Result<AuthToken>> RefreshTokenAsync(string accessToken, string refreshToken);
    Task<string?> GetUserNameAsync(Guid userId);
    Task<bool> IsInRoleAsync(Guid userId, string role);
    Task<Result<(Guid UserId, string FirstName, string LastName, string Email)>> GetUserAsync(Guid userId);
    Task<bool> AuthorizeAsync(Guid userId, string policyName);
    Task<Result> RevokeAsync(string email);
    Task<Result> RevokeAllAsync();
}