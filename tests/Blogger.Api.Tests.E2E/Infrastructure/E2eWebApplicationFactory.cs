using Blogger.Api.Controllers;
using Blogger.Domain;
using Blogger.Domain.Abstractions;
using Blogger.Domain.Models.Authors;
using Blogger.Domain.Models.Posts;
using Blogger.Domain.UseCases.Posts.GetPostById;
using Blogger.Persistence.Data;
using Blogger.Persistence.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Blogger.Api.Tests.E2E.Infrastructure;

public sealed class E2eWebApplicationFactory : WebApplicationFactory<AuthorsController>
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("E2E");

        builder.ConfigureTestServices(services =>
        {
            services.AddDbContext<BloggerDbContext>(options =>
                options
                    .UseInMemoryDatabase(_databaseName)
                    .ConfigureWarnings(warnings =>
                        warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)));

            services.AddScoped<IUnitOfWork, PassthroughUnitOfWork>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IPostDetailsRepository, PostDetailsRepository>();

            services.AddBloggerDomain();
        });
    }
}
