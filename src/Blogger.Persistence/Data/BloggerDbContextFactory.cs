using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Blogger.Persistence.Data;

public class BloggerDbContextFactory : IDesignTimeDbContextFactory<BloggerDbContext>
{
    public BloggerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BloggerDbContext>();
        optionsBuilder.UseMySQL(
            "Server=localhost;Port=3306;Database=blogger;User=blogger;Password=blogger;");

        return new BloggerDbContext(optionsBuilder.Options);
    }
}
