using System.Diagnostics;
using System.Text;
using System.Xml.Serialization;
using Blogger.Api.XmlContracts.ProblemDetails;
using Microsoft.AspNetCore.Mvc;

namespace Blogger.Api.Infrastructure;

internal static class ProblemDetailsResponseWriter
{
    public static async Task WriteAsync(
        HttpContext httpContext,
        ProblemDetails problemDetails,
        CancellationToken cancellationToken)
    {
        problemDetails.Extensions["traceId"] =
            Activity.Current?.Id ?? httpContext.TraceIdentifier;

        if (PrefersXml(httpContext))
        {
            httpContext.Response.ContentType = "application/xml; charset=utf-8";
            await httpContext.Response.WriteAsync(
                SerializeProblemDetails(problemDetails),
                cancellationToken);
            return;
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }

    public static async Task WriteValidationAsync(
        HttpContext httpContext,
        ValidationProblemDetails problemDetails,
        CancellationToken cancellationToken)
    {
        problemDetails.Extensions["traceId"] =
            Activity.Current?.Id ?? httpContext.TraceIdentifier;

        if (PrefersXml(httpContext))
        {
            httpContext.Response.ContentType = "application/xml; charset=utf-8";
            await httpContext.Response.WriteAsync(
                SerializeValidationProblemDetails(problemDetails),
                cancellationToken);
            return;
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }

    private static bool PrefersXml(HttpContext httpContext) =>
        httpContext.Request.Path.StartsWithSegments("/api/xml")
        || httpContext.Request.Headers.Accept.Any(value =>
            value?.Contains("application/xml", StringComparison.OrdinalIgnoreCase) == true);

    private static string SerializeProblemDetails(ProblemDetails problemDetails)
    {
        var response = new ProblemDetailsXmlResponse
        {
            Title = problemDetails.Title,
            Status = problemDetails.Status,
            Detail = problemDetails.Detail,
            Type = problemDetails.Type,
            Instance = problemDetails.Instance,
            TraceId = problemDetails.Extensions.TryGetValue("traceId", out var traceId)
                ? traceId?.ToString()
                : null
        };

        return Serialize(response);
    }

    private static string SerializeValidationProblemDetails(ValidationProblemDetails problemDetails)
    {
        var response = new ValidationProblemDetailsXmlResponse
        {
            Title = problemDetails.Title,
            Status = problemDetails.Status,
            Type = problemDetails.Type,
            Instance = problemDetails.Instance,
            TraceId = problemDetails.Extensions.TryGetValue("traceId", out var traceId)
                ? traceId?.ToString()
                : null,
            Errors = problemDetails.Errors
                .SelectMany(pair => pair.Value.Select(message => new ValidationErrorXmlResponse
                {
                    PropertyName = pair.Key,
                    Message = message
                }))
                .ToList()
        };

        return Serialize(response);
    }

    private static string Serialize<T>(T value)
    {
        var serializer = new XmlSerializer(typeof(T));
        using var writer = new StringWriter(new StringBuilder());
        serializer.Serialize(writer, value);
        return writer.ToString();
    }
}
