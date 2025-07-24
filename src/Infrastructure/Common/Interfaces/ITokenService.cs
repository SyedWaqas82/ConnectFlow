using System.Security.Claims;
using ConnectFlow.Infrastructure.Identity;

namespace ConnectFlow.Infrastructure.Common.Interfaces;

public interface ITokenService
{
    Task<(string AccessToken, int ExpiresInMinutes)> CreateAccessTokenAsync(ApplicationUser user);
    (string RefreshToken, DateTimeOffset Expiry) CreateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string accessToken);
}