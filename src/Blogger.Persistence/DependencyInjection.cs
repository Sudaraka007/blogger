using Blogger.Domain;
using Blogger.Domain.Abstractions;
using Blogger.Domain.Models.Authors;
using Blogger.Domain.Models.Posts;
using Blogger.Domain.UseCases.Posts.GetPostById;
using Blogger.Persistence.Data;
using Blogger.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Blogger.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddBloggerPersistence(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<BloggerDbContext>(options =>
            options.UseMySQL(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IPostDetailsRepository, PostDetailsRepository>();

        services.AddBloggerDomain();

        return services;
    }
}
