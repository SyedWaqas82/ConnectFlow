namespace ConnectFlow.Application.Subscriptions.Commands.CreatePortalSession;

public class CreateBillingPortalSessionCommandValidator : AbstractValidator<CreateBillingPortalSessionCommand>
{
    public CreateBillingPortalSessionCommandValidator()
    {
        RuleFor(x => x.ReturnUrl)
            .NotEmpty()
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("ReturnUrl must be a valid absolute URI.");
    }
}