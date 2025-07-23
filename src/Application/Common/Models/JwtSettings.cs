namespace ConnectFlow.Application.Identity;

public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public double AccessTokenExpirationMinutes { get; set; } = 15;
    public double RefreshTokenExpirationDays { get; set; } = 7;
}
