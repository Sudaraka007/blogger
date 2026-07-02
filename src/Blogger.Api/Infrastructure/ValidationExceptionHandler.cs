using Blogger.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Blogger.Api.Infrastructure;

public sealed class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var errors = exception switch
        {
            ValidationException validationException => validationException.Errors
                .Select(error => new DomainValidationFailure(error.PropertyName, error.ErrorMessage))
                .ToList(),
            DomainValidationException domainValidationException => domainValidationException.Failures,
            _ => null
        };

        if (errors is null)
        {
            return false;
        }

        var problemDetails = new ValidationProblemDetails(
            errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray()))
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
}
