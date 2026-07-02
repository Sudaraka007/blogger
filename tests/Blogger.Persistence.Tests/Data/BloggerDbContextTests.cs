using Blogger.Persistence.Data;
using Blogger.Persistence.Entities;
using Blogger.Persistence.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Blogger.Persistence.Tests.Data;

public sealed class BloggerDbContextTests : IDisposable
{
    private readonly PersistenceTestContext _context = new();

    [Fact]
    public async Task Can_persist_author_and_post_with_relationship()
    {
        var author = new Author
        {
            Name = "John",
            Surname = "Doe"
        };

        var post = new Post
        {
            Author = author,
            Title = "Title",
            Description = "Description",
            Content = "Content"
        };

        _context.DbContext.Authors.Add(author);
        _context.DbContext.Posts.Add(post);
        await _context.DbContext.SaveChangesAsync();

        var loaded = await _context.DbContext.Posts
            .Include(p => p.Author)
            .SingleAsync();

        Assert.True(loaded.Id > 0);
        Assert.Equal(author.Id, loaded.AuthorId);
        Assert.Equal("John", loaded.Author.Name);
        Assert.False(loaded.Removed);
        Assert.False(loaded.Author.Removed);
    }

    [Fact]
    public async Task Author_posts_navigation_loads_related_posts()
    {
        var author = await SeedData.AuthorAsync(_context.DbContext);
        await SeedData.PostAsync(_context.DbContext, author.Id, "First");
        await SeedData.PostAsync(_context.DbContext, author.Id, "Second");

        var loaded = await _context.DbContext.Authors
            .Include(a => a.Posts)
            .SingleAsync();

        Assert.Equal(2, loaded.Posts.Count);
    }

    public void Dispose() => _context.Dispose();
}
