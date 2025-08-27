namespace ConnectFlow.Application.Users.Commands.UpdatePassword;

public class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
{
    public UpdatePasswordCommandValidator()
    {
        RuleFor(x => x.ApplicationUserPublicId)
            .NotEmpty().WithMessage("Public User ID is required.");

        RuleFor(x => x.PasswordToken)
            .NotEmpty().WithMessage("Password token is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(6).WithMessage("New password must be at least 6 characters long.");
    }
}