using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Blogger.Api.Infrastructure;

public sealed class RequestValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        foreach (var parameter in context.ActionDescriptor.Parameters)
        {
            if (parameter.ParameterType == typeof(CancellationToken))
            {
                continue;
            }

            var isBodyParameter = parameter.BindingInfo?.BindingSource is { } bindingSource
                && bindingSource.CanAcceptDataFrom(BindingSource.Body);

            context.ActionArguments.TryGetValue(parameter.Name!, out var argument);

            if (isBodyParameter && argument is null)
            {
                throw new ValidationException(
                [
                    new ValidationFailure(
                        parameter.Name!,
                        "Request body is required.")
                ]);
            }

            if (argument is null)
            {
                continue;
            }

            var argumentType = argument.GetType();
            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

            if (context.HttpContext.RequestServices.GetService(validatorType) is not IValidator validator)
            {
                continue;
            }

            var validationContext = new ValidationContext<object>(argument);
            var result = await validator.ValidateAsync(
                validationContext,
                context.HttpContext.RequestAborted);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }

        await next();
    }
}
