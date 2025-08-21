using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Application.Users.Commands.ConfirmEmail;

public class ConfirmEmailCommand : IRequest<Result>
{
    public Guid UserId { get; init; }
    public string ConfirmationToken { get; init; } = string.Empty;
}

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result>
{
    private readonly IIdentityService _identityService;

    public ConfirmEmailCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.ConfirmEmailAsync(request.UserId, request.ConfirmationToken);
    }
}