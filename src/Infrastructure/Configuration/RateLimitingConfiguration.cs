using System.Diagnostics.Metrics;
using System.Threading.RateLimiting;
using ConnectFlow.Application.Common.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Hosting;

namespace ConnectFlow.Infrastructure.Configuration;

public static class RateLimitingConfiguration
{
    /// <summary>
    /// Adds rate limiting capabilities to protect API endpoints
    /// </summary>
    /// <remarks>
    /// For complete documentation on rate limiting options, metrics, and 
    /// dashboard links, please see monitoring/README.md
    /// </remarks>

    // Create a static meter to ensure metrics persist between requests
    private static readonly Meter _meter = new("ConnectFlow.Metrics");
    private static readonly Counter<int> _rateLimitExceededCounter = _meter.CreateCounter<int>("rate_limit_exceeded_total", "requests", "Number of requests that exceeded rate limits");

    public static void AddRateLimiting(this IHostApplicationBuilder builder)
    {
        var rateLimitSettings = builder.Configuration
            .GetSection("RateLimiting")
            .Get<RateLimitingSettings>() ?? new RateLimitingSettings();

        builder.Services.AddRateLimiter(options =>
        {
            if (rateLimitSettings.Enabled)
            {
                ConfigureGlobalLimiter(options, rateLimitSettings);
                ConfigureApiPolicy(options, builder, rateLimitSettings);
            }

            options.OnRejected = async (context, token) =>
            {
                await HandleRejectedRequestAsync(context, token);
            };
        });

        builder.Services.Configure<RateLimitingSettings>(builder.Configuration.GetSection("RateLimiting"));
    }

    private static void ConfigureGlobalLimiter(RateLimiterOptions options, RateLimitingSettings settings)
    {
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        {
            var clientId = context.Connection.RemoteIpAddress?.ToString() ?? context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? "anonymous";

            return RateLimitPartition.GetFixedWindowLimiter(clientId, _ =>
                new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = settings.FixedWindow.PermitLimit,
                    Window = TimeSpan.FromSeconds(settings.FixedWindow.WindowSeconds)
                });
        });
    }

    private static void ConfigureApiPolicy(RateLimiterOptions options, IHostApplicationBuilder builder, RateLimitingSettings settings)
    {
        options.AddPolicy("api", context =>
        {
            var tenantHeader = builder.Configuration["TenantSettings:HeaderName"] ?? "X-Tenant-Id";
            var tenantId = context.Request.Headers[tenantHeader].ToString() ?? "default";

            return RateLimitPartition.GetTokenBucketLimiter(tenantId, _ =>
                new TokenBucketRateLimiterOptions
                {
                    TokenLimit = settings.TokenBucket.TokenLimit,
                    QueueLimit = 10,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                    TokensPerPeriod = settings.TokenBucket.TokensPerPeriod,
                    AutoReplenishment = true
                });
        });
    }

    private static async Task HandleRejectedRequestAsync(OnRejectedContext context, CancellationToken token)
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.Headers["Retry-After"] = "60";

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "Too many requests. Please try again later.",
            retryAfter = 60
        }, token);

        var path = context.HttpContext.Request.Path.Value ?? "/";
        var endpoint = path.Split('/').LastOrDefault() ?? "unknown";

        _rateLimitExceededCounter.Add(1, new KeyValuePair<string, object?>("path", path), new KeyValuePair<string, object?>("endpoint", endpoint));
    }
}