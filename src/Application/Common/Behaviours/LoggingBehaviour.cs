using ConnectFlow.Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace ConnectFlow.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger _logger;
    private readonly IContextService _contextService;

    public LoggingBehaviour(ILogger<TRequest> logger, IContextService contextService)
    {
        _logger = logger;
        _contextService = contextService;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = await Task.FromResult(_contextService.GetCurrentPublicUserId());

        string? userName = _contextService.GetCurrentUserName();

        _logger.LogInformation("ConnectFlow Request: {Name} {@UserId} {@UserName} {@Request}", requestName, userId, userName, request);
    }
}