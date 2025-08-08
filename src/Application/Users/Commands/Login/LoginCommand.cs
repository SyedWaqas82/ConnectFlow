using System;
using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Application.Users.Commands.Login;

public class LoginCommand : IRequest<Result<AuthToken>>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthToken>>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<AuthToken>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.SignInAsync(request.Email, request.Password);
    }
}