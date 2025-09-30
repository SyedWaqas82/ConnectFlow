namespace ConnectFlow.Domain.Enums;

public enum AssignmentRuleField
{
    // Lead/Deal common fields
    Value = 1,
    Currency = 2,
    SourceChannel = 3,
    SourceOrigin = 4,
    ExpectedCloseDate = 5,
    Title = 6,

    // Organization fields
    OrganizationName = 10,
    OrganizationIndustry = 11,
    OrganizationNumberOfEmployees = 12,
    OrganizationAnnualRevenue = 13,
    OrganizationWebsite = 14,

    // Person fields
    PersonName = 20,

    // Deal specific fields
    PipelineId = 30,
    PipelineStageId = 31,
    Probability = 32,

    // Contact information
    PersonEmail = 40,
    PersonPhone = 41,
    OrganizationAddress = 42,

    // Time-based fields
    CreatedAt = 50,
    UpdatedAt = 51,
}

public enum AssignmentRuleEntityType
{
    Lead = 1,
    Deal = 2,
}

public enum AssignmentTriggerEvent
{
    OnCreate = 1,
    OnUpdate = 2
}

public enum AssignmentRuleExecutionResult
{
    Success = 1,
    Failed = 2,
    NoMatch = 3,
    Error = 4,
    Skipped = 5
}