using Blogger.Domain;
using Blogger.Domain.Abstractions;
using Blogger.Domain.Models.Authors;
using Blogger.Domain.Models.Posts;
using Blogger.Domain.UseCases.Posts.GetPostById;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Blogger.Api.Tests.Unit.Mediator;

public sealed class HandlerRegistrationTests
{
    public static TheoryData<Type> RequestTypes()
    {
        var data = new TheoryData<Type>();

        foreach (var requestType in DomainRequestTypes())
        {
            data.Add(requestType);
        }

        return data;
    }

    [Fact]
    public void Domain_assembly_exposes_request_types()
    {
        Assert.NotEmpty(DomainRequestTypes());
    }

    [Theory]
    [MemberData(nameof(RequestTypes))]
    public void Every_request_has_a_registered_handler(Type requestType)
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();

        var responseType = ResponseType(requestType);
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

        var handler = scope.ServiceProvider.GetService(handlerType);

        Assert.True(
            handler is not null,
            $"No handler registered for {requestType.Name} (expected {handlerType}).");
    }

    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddBloggerDomain();

        services.AddScoped(_ => Mock.Of<IAuthorRepository>());
        services.AddScoped(_ => Mock.Of<IPostRepository>());
        services.AddScoped(_ => Mock.Of<IPostDetailsRepository>());
        services.AddScoped(_ => Mock.Of<IUnitOfWork>());

        return services.BuildServiceProvider(validateScopes: true);
    }

    private static IReadOnlyList<Type> DomainRequestTypes() =>
        typeof(Blogger.Domain.DependencyInjection).Assembly
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false })
            .Where(type => type.GetInterfaces().Any(IsRequestInterface))
            .ToList();

    private static Type ResponseType(Type requestType)
    {
        var requestInterface = requestType.GetInterfaces().First(IsRequestInterface);
        return requestInterface.GetGenericArguments()[0];
    }

    private static bool IsRequestInterface(Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IRequest<>);
}
