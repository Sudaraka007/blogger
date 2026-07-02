using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Blogger.Api.Infrastructure;

internal static class ProblemDetailsResponseWriter
{
    public static Task WriteAsync(
        HttpContext httpContext,
        ProblemDetails problemDetails,
        CancellationToken cancellationToken)
    {
        problemDetails.Extensions["traceId"] =
            Activity.Current?.Id ?? httpContext.TraceIdentifier;

        return httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }

    public static Task WriteValidationAsync(
        HttpContext httpContext,
        ValidationProblemDetails problemDetails,
        CancellationToken cancellationToken)
    {
        problemDetails.Extensions["traceId"] =
            Activity.Current?.Id ?? httpContext.TraceIdentifier;

        return httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }
}
