using System.Security.Claims;
using ConnectFlow.Infrastructure.Identity;

namespace ConnectFlow.Infrastructure.Common.Interfaces;

public interface IAuthTokenService
{
    Task<(string AccessToken, int ExpiresInMinutes)> CreateAccessTokenAsync(ApplicationUser appUser);
    (string RefreshToken, DateTimeOffset Expiry) CreateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string accessToken);
}