using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Application.Users.Commands;

public class ResetPasswordCommand : IRequest<Result<UserToken>>
{
    public string Email { get; init; } = string.Empty;
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<UserToken>>
{
    private readonly IIdentityService _identityService;

    public ResetPasswordCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<UserToken>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.ResetPasswordTokenAsync(request.Email);
    }
}