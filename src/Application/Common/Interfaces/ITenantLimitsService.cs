namespace ConnectFlow.Application.Common.Interfaces;

public interface ITenantLimitsService
{
    /// <summary>
    /// Checks if the tenant has reached its limit for the specified entity
    /// </summary>
    /// <param name="tenantId">The tenant ID</param>
    /// <param name="entityType">The type of entity to check limits for</param>
    /// <returns>True if the tenant has not reached its limit, false otherwise</returns>
    Task<bool> CanAddEntityAsync(int tenantId, Type entityType);

    /// <summary>
    /// Checks if the tenant can add more users based on subscription limits
    /// </summary>
    /// <param name="tenantId">The tenant ID</param>
    /// <returns>True if the tenant has not reached its user limit, false otherwise</returns>
    Task<bool> CanAddUserAsync(int tenantId);

    /// <summary>
    /// Checks if the tenant can use more AI tokens based on subscription limits
    /// </summary>
    /// <param name="tenantId">The tenant ID</param>
    /// <param name="tokenCount">The number of tokens to check</param>
    /// <returns>True if the tenant has not reached its AI token limit, false otherwise</returns>
    Task<bool> CanUseAiTokensAsync(int tenantId, int tokenCount);
}