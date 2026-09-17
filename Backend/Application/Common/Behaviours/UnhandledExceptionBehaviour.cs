using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours;

/// <summary>
/// MediatR pipeline behaviour that logs unhandled exceptions before re-throwing.
/// </summary>
public sealed class UnhandledExceptionBehaviour<TRequest, TResponse>(
    ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            logger.LogError(ex, "Unhandled exception for request {RequestName}: {@Request}", requestName, request);
            throw;
        }
    }
}
