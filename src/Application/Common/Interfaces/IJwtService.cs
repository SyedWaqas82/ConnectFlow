using ConnectFlow.Domain.Identity;

namespace ConnectFlow.Application.Common.Interfaces;

public interface IJwtService
{
    Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(ApplicationUser user);
    Task<(string AccessToken, string RefreshToken)?> RefreshTokenAsync(string accessToken, string refreshToken);
}
