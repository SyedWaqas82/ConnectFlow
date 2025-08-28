using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<UserToken>> CreateTenantForNewUserAsync(string email, string password, string firstName, string lastName, string? jobTitle = null, string? phoneNumber = null, string? mobile = null, string? timeZone = null, string? locale = null);
    Task<Result> CreateTenantForExistingUserAsync(string email);
    Task<Result<UserToken>> JoinTenantAsNewUserAsync(string email, string password, string firstName, string lastName, string[] roles, string? jobTitle = null, string? phoneNumber = null, string? mobile = null, string? timeZone = null, string? locale = null, int? tenantId = null);
    Task<Result> JoinTenantAsExistingUserAsync(string email, string[] roles, int tenantId);
    Task<Result> ConfirmEmailAsync(Guid applicationUserPublicId, string confirmationToken);
    Task<Result<UserToken>> ResetPasswordAsync(string email);
    Task<Result> UpdatePasswordAsync(Guid applicationUserPublicId, string passwordToken, string newPassword);
    Task<Result> ChangePasswordAsync(Guid applicationUserPublicId, string password, string newPassword);
    Task<Result<AuthToken>> SignInAsync(string email, string password);
    Task<Result<AuthToken>> RefreshTokenAsync(string accessToken, string refreshToken);
    Task<string?> GetUserNameAsync(Guid applicationUserPublicId);
    Task<bool> IsInRoleAsync(Guid applicationUserPublicId, string role);
    Task<Result<(Guid ApplicationUserPublicId, string FirstName, string LastName, string Email)>> GetUserAsync(Guid applicationUserPublicId);
    Task<Result<(Guid ApplicationUserPublicId, string FirstName, string LastName, string Email)>> GetUserAsync(int applicationUserId);
    Task<bool> AuthorizeAsync(Guid applicationUserPublicId, string policyName);
    Task<Result> RevokeAsync(string email);
    Task<Result> RevokeAllAsync();
    Task<Result> CreateStripeCustomerForExistingTenantAsync(int tenantId);
}