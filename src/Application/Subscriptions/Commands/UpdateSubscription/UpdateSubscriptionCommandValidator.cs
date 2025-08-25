namespace ConnectFlow.Application.Subscriptions.Commands.UpdateSubscription;

public class UpdateSubscriptionCommandValidator : AbstractValidator<UpdateSubscriptionCommand>
{
    public UpdateSubscriptionCommandValidator()
    {
        RuleFor(x => x.NewPlanId).NotEmpty().WithMessage("New plan ID is required.");
    }
}