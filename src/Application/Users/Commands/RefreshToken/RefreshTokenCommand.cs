using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Application.Users.Commands.RefreshToken;

public record RefreshTokenCommand : IRequest<Result<AuthToken>>
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthToken>>
{
    private readonly IIdentityService _identityService;

    public RefreshTokenCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<AuthToken>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.RefreshTokenAsync(request.AccessToken, request.RefreshToken);
    }
}