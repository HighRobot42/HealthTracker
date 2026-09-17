using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours;

/// <summary>
/// MediatR pipeline behaviour that warns when a request exceeds the threshold (500 ms).
/// </summary>
public sealed class PerformanceBehaviour<TRequest, TResponse>(
    ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const int WarningThresholdMs = 500;
    private readonly Stopwatch _timer = new();

    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Restart();
        var response = await next();
        _timer.Stop();

        if (_timer.ElapsedMilliseconds > WarningThresholdMs)
            logger.LogWarning("Slow request detected: {RequestName} ({Elapsed}ms) {@Request}",
                typeof(TRequest).Name, _timer.ElapsedMilliseconds, request);

        return response;
    }
}
