using MediatR.Pipeline;

namespace ConnectFlow.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger _logger;
    private readonly IContextManager _contextManager;

    public LoggingBehaviour(ILogger<TRequest> logger, IContextManager contextManager)
    {
        _logger = logger;
        _contextManager = contextManager;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = await Task.FromResult(_contextManager.GetCurrentUserId());

        string? userName = _contextManager.GetCurrentUserName();

        _logger.LogInformation("ConnectFlow Request: {Name} {@UserId} {@UserName} {@Request}", requestName, userId, userName, request);
    }
}