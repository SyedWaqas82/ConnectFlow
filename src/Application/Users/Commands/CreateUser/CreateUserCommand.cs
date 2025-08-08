using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Application.Users.Commands;

public class CreateUserCommand : IRequest<Result<UserToken>>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserToken>>
{
    private readonly IIdentityService _identityService;

    public CreateUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<UserToken>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.CreateTenantForNewUserAsync(request.Email, request.Password, request.FirstName, request.LastName);
    }
}