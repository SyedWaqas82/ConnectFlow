using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Application.Users.Commands.UpdatePassword;

public class UpdatePasswordCommand : IRequest<Result>
{
    public Guid UserId { get; init; }
    public string PasswordToken { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}

public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, Result>
{
    private readonly IIdentityService _identityService;

    public UpdatePasswordCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.UpdatePasswordAsync(request.UserId, request.PasswordToken, request.NewPassword);
    }
}
