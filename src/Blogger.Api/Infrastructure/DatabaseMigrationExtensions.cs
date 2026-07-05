using Blogger.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Blogger.Api.Infrastructure;

internal static class DatabaseMigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        if (app.Environment.IsEnvironment("E2E") || app.Environment.IsEnvironment("Testing"))
        {
            return;
        }

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BloggerDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
