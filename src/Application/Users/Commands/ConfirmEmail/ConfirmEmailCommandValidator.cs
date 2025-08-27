namespace ConnectFlow.Application.Users.Commands.ConfirmEmail;

public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(x => x.ApplicationUserPublicId)
            .NotEmpty().WithMessage("Public User ID is required.");

        RuleFor(x => x.ConfirmationToken)
            .NotEmpty().WithMessage("Confirmation token is required.");
    }
}