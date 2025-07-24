namespace ConnectFlow.Application.Common.Models;

public class AuthToken
{
    public Guid ApplicationUserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public int ExpiresIn { get; init; }
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
}