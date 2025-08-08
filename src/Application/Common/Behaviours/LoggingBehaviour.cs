using ConnectFlow.Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace ConnectFlow.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger _logger;
    private readonly ICurrentUserService _currentUserService;

    public LoggingBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = await Task.FromResult(_currentUserService.GetCurrentUserId());

        string? userName = _currentUserService.GetCurrentUserName();

        _logger.LogInformation("ConnectFlow Request: {Name} {@UserId} {@UserName} {@Request}", requestName, userId, userName, request);
    }
}