using System.Reflection;
using Blogger.Api.Controllers;
using Blogger.Domain;
using Blogger.Domain.Abstractions;
using Blogger.Domain.Models.Authors;
using Blogger.Domain.Models.Posts;
using Blogger.Domain.UseCases.Posts.GetPostById;
using Blogger.Persistence.Data;
using Blogger.Persistence.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Blogger.Api.Tests.Unit.DependencyInjection;

public sealed class DependencyResolutionTests
{
    public static TheoryData<Type> InjectableTypes()
    {
        var data = new TheoryData<Type>();

        foreach (var type in HandlerTypes()
                     .Concat(ControllerTypes())
                     .Concat(PersistenceServiceTypes()))
        {
            data.Add(type);
        }

        return data;
    }

    [Fact]
    public void Application_exposes_injectable_types()
    {
        Assert.NotEmpty(InjectableTypes());
    }

    [Theory]
    [MemberData(nameof(InjectableTypes))]
    public void Every_constructor_dependency_is_registered(Type injectableType)
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();

        var constructor = injectableType
            .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .Single();

        foreach (var parameter in constructor.GetParameters())
        {
            var service = scope.ServiceProvider.GetService(parameter.ParameterType);

            Assert.True(
                service is not null,
                $"{injectableType.Name} requires {parameter.ParameterType.Name}, but it is not registered.");
        }
    }

    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();

        services.AddLogging();

        services.AddDbContext<BloggerDbContext>(options =>
            options
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IPostDetailsRepository, PostDetailsRepository>();

        services.AddBloggerDomain();

        return services.BuildServiceProvider(validateScopes: true);
    }

    private static IEnumerable<Type> HandlerTypes() =>
        typeof(Blogger.Domain.DependencyInjection).Assembly
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false })
            .Where(type => type.GetInterfaces().Any(interfaceType =>
                interfaceType.IsGenericType
                && interfaceType.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)));

    private static IEnumerable<Type> ControllerTypes() =>
        typeof(AuthorsController).Assembly
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false })
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type));

    private static IEnumerable<Type> PersistenceServiceTypes() =>
    [
        typeof(AuthorRepository),
        typeof(PostRepository),
        typeof(PostDetailsRepository),
        typeof(UnitOfWork)
    ];
}
