using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ConnectFlow.Infrastructure.Identity;
using ConnectFlow.Infrastructure.Common.Interfaces;
using System.Security.Cryptography;
using ConnectFlow.Infrastructure.Common.Models;

namespace ConnectFlow.Infrastructure.Services;

public class AuthTokenService : IAuthTokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthTokenService(IOptions<JwtSettings> jwtSettings, UserManager<ApplicationUser> userManager)
    {
        _jwtSettings = jwtSettings.Value;
        _userManager = userManager;
    }

    public async Task<(string AccessToken, int ExpiresInMinutes)> CreateAccessTokenAsync(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        // Use a consistent DateTimeOffset for all time-related operations
        // Get the current UTC time as the basis for all time calculations
        DateTimeOffset utcNow = DateTimeOffset.UtcNow;

        // Convert to Unix timestamp in seconds for the token claims
        long unixTimeSeconds = utcNow.ToUnixTimeSeconds();

        // Calculate expiration time
        DateTimeOffset expirationTime = utcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

        var claims = new List<Claim>
        {
                new Claim(JwtRegisteredClaimNames.Sub, user.PublicId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, unixTimeSeconds.ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.NameIdentifier, user.PublicId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.Role, string.Join(",", userRoles))
        };

        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)), SecurityAlgorithms.HmacSha256);

        // Create the JWT token with consistent time values
        JwtSecurityToken jwtSecurityToken = new(issuer: _jwtSettings.Issuer, audience: _jwtSettings.Audience, claims: claims, notBefore: utcNow.UtcDateTime, expires: expirationTime.UtcDateTime, signingCredentials: signingCredentials);

        return (new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken), _jwtSettings.AccessTokenExpirationMinutes);
    }

    public (string RefreshToken, DateTimeOffset Expiry) CreateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        // Use DateTimeOffset for refresh token expiry
        var now = DateTimeOffset.UtcNow;
        var expiry = now.AddDays(_jwtSettings.RefreshTokenExpirationDays);
        return (Convert.ToBase64String(randomNumber), expiry);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, // Don't validate audience when checking expired tokens
            ValidateIssuer = false,   // Don't validate issuer when checking expired tokens
            ValidateIssuerSigningKey = true, // Still validate the signing key
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
            ValidateLifetime = false, // Don't validate lifetime since we're explicitly checking an expired token
            ClockSkew = TimeSpan.Zero // Use zero clock skew for exact time validation
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}