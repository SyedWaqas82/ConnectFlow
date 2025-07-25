﻿using System.Diagnostics;
using ConnectFlow.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConnectFlow.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly Stopwatch _timer;
    private readonly ILogger<TRequest> _logger;
    private readonly IUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public PerformanceBehaviour(
        ILogger<TRequest> logger,
        IUserService currentUserService,
        IIdentityService identityService)
    {
        _timer = new Stopwatch();

        _logger = logger;
        _currentUserService = currentUserService;
        _identityService = identityService;
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
            var userId = _currentUserService.PublicUserId;
            var userName = string.Empty;

            if (userId.HasValue)
            {
                userName = await _identityService.GetUserNameAsync(userId.Value);
            }

            _logger.LogWarning("ConnectFlow Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}",
                requestName, elapsedMilliseconds, userId, userName, request);
        }

        return response;
    }
}
