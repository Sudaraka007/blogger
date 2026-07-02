using MediatR;
using Microsoft.Extensions.Logging;

namespace Blogger.Domain.Abstractions;

public sealed class MonitoringBehavior<TRequest, TResponse>(ILogger<MonitoringBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IMonitoredCommand monitoredCommand)
        {
            return await next(cancellationToken);
        }

        try
        {
            var response = await next(cancellationToken);
            monitoredCommand.LogSuccess(logger);
            return response;
        }
        catch (Exception exception)
        {
            monitoredCommand.LogFailure(logger, exception);
            throw;
        }
    }
}
