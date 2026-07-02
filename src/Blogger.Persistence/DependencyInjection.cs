using Blogger.Domain.Authors;
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

        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IPostRepository, PostRepository>();

        return services;
    }
}
