namespace ConnectFlow.Infrastructure.Common.Models;

/// <summary>
/// Configuration settings for API rate limiting
/// </summary>
public class RateLimitingSettings
{
    public const string SectionName = "RateLimiting";
    public bool Enabled { get; set; } = true;
    public FixedWindowSettings FixedWindow { get; set; } = new();
    public TokenBucketSettings TokenBucket { get; set; } = new();
}

/// <summary>
/// Fixed window rate limiting configuration
/// </summary>
public class FixedWindowSettings
{
    public int PermitLimit { get; set; } = 100;
    public int WindowSeconds { get; set; } = 60;
}

/// <summary>
/// Token bucket rate limiting configuration
/// </summary>
public class TokenBucketSettings
{
    public int TokenLimit { get; set; } = 50;
    public int TokensPerPeriod { get; set; } = 10;
}