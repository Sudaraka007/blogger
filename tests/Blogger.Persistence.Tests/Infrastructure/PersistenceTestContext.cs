using Blogger.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Blogger.Persistence.Tests.Infrastructure;

public sealed class PersistenceTestContext : IDisposable
{
    public PersistenceTestContext()
    {
        var options = new DbContextOptionsBuilder<BloggerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        DbContext = new BloggerDbContext(options);
    }

    public BloggerDbContext DbContext { get; }

    public void Dispose() => DbContext.Dispose();
}
