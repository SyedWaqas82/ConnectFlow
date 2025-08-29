namespace ConnectFlow.Application.Subscriptions.Queries.GetCheckoutSession;

public class GetCheckoutSessionQueryValidator : AbstractValidator<GetCheckoutSessionQuery>
{
    public GetCheckoutSessionQueryValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty()
            .WithMessage("Session ID is required")
            .Must(BeValidStripeSessionId)
            .WithMessage("Session ID must be a valid Stripe checkout session ID (starts with 'cs_')");
    }

    private static bool BeValidStripeSessionId(string sessionId)
    {
        return !string.IsNullOrWhiteSpace(sessionId) && sessionId.StartsWith("cs_");
    }
}