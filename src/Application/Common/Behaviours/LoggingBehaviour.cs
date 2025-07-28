using ConnectFlow.Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace ConnectFlow.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger _logger;
    private readonly IUserService _currentUserService;

    public LoggingBehaviour(ILogger<TRequest> logger, IUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService.PublicUserId;
        string? userName = string.Empty;

        if (userId.HasValue)
        {
            userName = await Task.FromResult(_currentUserService.GetUserName());
        }

        _logger.LogInformation("ConnectFlow Request: {Name} {@UserId} {@UserName} {@Request}", requestName, userId, userName, request);
    }
}