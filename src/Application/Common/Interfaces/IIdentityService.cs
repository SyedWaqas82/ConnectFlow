using ConnectFlow.Application.Common.Models;
using ConnectFlow.Domain.Enums;

namespace ConnectFlow.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<UserToken>> CreateTenantForNewUserAsync(string code, string domain, string name, string description, string settings, string email, string password, string firstName, string lastName, string? jobTitle, string? phoneNumber, string? mobile, string? timeZone, string? locale, SubscriptionPlan plan = SubscriptionPlan.Free);
    Task<Result> CreateTenantForExistingUserAsync(string code, string domain, string name, string description, string settings, string email, SubscriptionPlan plan = SubscriptionPlan.Free);
    Task<Result<UserToken>> JoinTenantAsNewUserAsync(string email, string password, string firstName, string lastName, string? jobTitle, string? phoneNumber, string? mobile, string? timeZone, string? locale, string[] roles, int? tenantId);
    Task<Result> JoinTenantAsExistingUserAsync(string email, string[] roles, int tenantId);
    Task<Result> ConfirmEmailAsync(Guid userId, string confirmationToken);
    Task<Result<UserToken>> ResetPasswordTokenAsync(string email);
    Task<Result> ResetPasswordAsync(Guid userId, string passwordToken, string newPassword);
    Task<Result> ChangePasswordAsync(Guid userId, string password, string newPassword);
    Task<Result<AuthToken>> SignInAsync(string email, string password);
    Task<Result<AuthToken>> RefreshTokenAsync(string accessToken, string refreshToken);
    Task<string?> GetUserNameAsync(Guid userId);
    Task<bool> IsInRoleAsync(Guid userId, string role);
    Task<bool> AuthorizeAsync(Guid userId, string policyName);
    Task<Result> RevokeAsync(string email);
    Task<Result> RevokeAllAsync();
}