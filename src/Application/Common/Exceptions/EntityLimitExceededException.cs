namespace ConnectFlow.Application.Common.Exceptions;

public class EntityLimitExceededException : Exception
{
    public LimitValidationType LimitType { get; }
    public int CurrentUsage { get; }
    public int AllowedLimit { get; }
    public string? UpgradeRecommendation { get; }
    public SubscriptionPlan? RecommendedPlan { get; }
    public decimal? RecommendedPlanPrice { get; }

    public EntityLimitExceededException(LimitValidationType limitType, int currentUsage, int allowedLimit, string? upgradeRecommendation = null, SubscriptionPlan? recommendedPlan = null, decimal? recommendedPlanPrice = null) : base($"You have reached the limit for {limitType} entities. Current: {currentUsage}, Allowed: {allowedLimit}. {upgradeRecommendation}")
    {
        LimitType = limitType;
        CurrentUsage = currentUsage;
        AllowedLimit = allowedLimit;
        UpgradeRecommendation = upgradeRecommendation;
        RecommendedPlan = recommendedPlan;
        RecommendedPlanPrice = recommendedPlanPrice;
    }
}