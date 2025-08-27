namespace ConnectFlow.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly Stopwatch _timer;
    private readonly ILogger<TRequest> _logger;
    private readonly IContextManager _contextManager;

    public PerformanceBehaviour(ILogger<TRequest> logger, IContextManager contextManager)
    {
        _timer = new Stopwatch();

        _logger = logger;
        _contextManager = contextManager;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            var applicationUserPublicId = _contextManager.GetCurrentApplicationUserPublicId();
            var userName = _contextManager.GetCurrentUserName();

            _logger.LogWarning("ConnectFlow Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@ApplicationUserPublicId} {@UserName} {@Request}", requestName, elapsedMilliseconds, applicationUserPublicId, userName, request);
        }

        return response;
    }
}