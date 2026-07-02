using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blogger.Api.Infrastructure;

public sealed class DbUpdateExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DbUpdateException dbUpdateException)
        {
            return false;
        }

        var message = dbUpdateException.InnerException?.Message
            ?? dbUpdateException.Message;

        if (IsForeignKeyViolation(message))
        {
            var problemDetails = new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                ["request"] = ["The request could not be completed because a related resource is missing or invalid."]
            })
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation failed.",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await ProblemDetailsResponseWriter.WriteValidationAsync(
                httpContext,
                problemDetails,
                cancellationToken);
            return true;
        }

        var serverProblem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "A database error occurred.",
            Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await ProblemDetailsResponseWriter.WriteAsync(
            httpContext,
            serverProblem,
            cancellationToken);
        return true;
    }

    private static bool IsForeignKeyViolation(string message) =>
        message.Contains("foreign key constraint", StringComparison.OrdinalIgnoreCase)
        || message.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase)
        || message.Contains("Cannot add or update a child row", StringComparison.OrdinalIgnoreCase);
}
