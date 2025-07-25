using ConnectFlow.Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace ConnectFlow.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger _logger;
    private readonly IUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public LoggingBehaviour(ILogger<TRequest> logger, IUserService currentUserService, IIdentityService identityService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService.PublicUserId;
        string? userName = string.Empty;

        if (userId.HasValue)
        {
            userName = await _identityService.GetUserNameAsync(userId.Value);
        }

        _logger.LogInformation("ConnectFlow Request: {Name} {@UserId} {@UserName} {@Request}",
            requestName, userId, userName, request);
    }
}
