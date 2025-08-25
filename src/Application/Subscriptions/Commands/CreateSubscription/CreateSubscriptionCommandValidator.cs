namespace ConnectFlow.Application.Subscriptions.Commands.CreateSubscription;

public class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        RuleFor(x => x.PlanId).NotEmpty();
        RuleFor(x => x.SuccessUrl)
            .NotEmpty()
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("SuccessUrl must be a valid absolute URI.");

        RuleFor(x => x.CancelUrl)
            .NotEmpty()
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("CancelUrl must be a valid absolute URI.");
    }
}