using ConnectFlow.Domain.Entities;

namespace ConnectFlow.Application.Common.Interfaces;

public interface IJwtService
{
    Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(TenantUser user);
    Task<(string AccessToken, string RefreshToken)?> RefreshTokenAsync(string accessToken, string refreshToken);
}
