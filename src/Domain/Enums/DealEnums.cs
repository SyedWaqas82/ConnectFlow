namespace ConnectFlow.Domain.Enums;

public enum DealStatus
{
    Open = 1,
    Won = 2,
    Lost = 3
}

public enum DiscountType
{
    FixedAmount = 1,
    Percentage = 2,
}

public enum BillingFrequency
{
    OneTime = 1,
    Daily = 2,
    Weekly = 3,
    Monthly = 4,
    Quarterly = 5,
    SemiAnnual = 6,
    Annual = 7,
    Custom = 99
}

public enum TaxType
{
    None = 1,
    Inclusive = 2,
    Exclusive = 3
}