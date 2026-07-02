using Blogger.Domain.Abstractions;
using Blogger.Domain.UseCases.Authors.CreateAuthor;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Blogger.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddBloggerDomain(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateAuthorCommandValidator>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<CreateAuthorCommandHandler>();
            cfg.AddOpenBehavior(typeof(MonitoringBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
        });

        return services;
    }
}
