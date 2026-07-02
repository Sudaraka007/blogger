using Blogger.Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Blogger.Api.Tests.Integration.Infrastructure;

/// <summary>
/// Integration tests mock <see cref="MediatR.IMediator"/> to exercise the HTTP/API layer in isolation.
/// MediatR pipeline behaviors and handlers are covered by domain tests and E2E tests.
/// </summary>
public sealed class BloggerWebApplicationFactory : WebApplicationFactory<AuthorsController>
{
    public Mock<IMediator> MediatorMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] =
                    "Server=localhost;Database=blogger_test;User=test;Password=test;"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IMediator>();
            services.AddSingleton(MediatorMock.Object);
        });
    }
}
