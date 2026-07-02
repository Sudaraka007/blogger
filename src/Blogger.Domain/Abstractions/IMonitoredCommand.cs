using Microsoft.Extensions.Logging;

namespace Blogger.Domain.Abstractions;

public interface IMonitoredCommand
{
    void LogSuccess(ILogger logger)
    {
        logger.LogInformation("{UseCase} succeeded: {@UseCase}", GetType().Name, this);
    }

    void LogFailure(ILogger logger, Exception exception)
    {
        logger.LogError(exception, "{UseCase} failed: {@UseCase}", GetType().Name, this);
    }
}
