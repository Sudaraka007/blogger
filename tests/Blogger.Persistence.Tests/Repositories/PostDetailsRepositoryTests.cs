using Blogger.Persistence.Repositories;
using Blogger.Persistence.Tests.Infrastructure;

namespace Blogger.Persistence.Tests.Repositories;

public sealed class PostDetailsRepositoryTests : IDisposable
{
    private readonly PersistenceTestContext _context = new();
    private readonly PostDetailsRepository _repository;

    public PostDetailsRepositoryTests()
    {
        _repository = new PostDetailsRepository(_context.DbContext);
    }

    [Fact]
    public async Task GetByIdAsync_returns_post_details_without_author()
    {
        var author = await SeedData.AuthorAsync(_context.DbContext);
        var post = await SeedData.PostAsync(_context.DbContext, author.Id, "Title", "Description", "Content");

        var details = await _repository.GetByIdAsync(post.Id, includeAuthor: false);

        Assert.NotNull(details);
        Assert.Equal("Title", details.Title);
        Assert.Equal("Description", details.Description);
        Assert.Equal("Content", details.Content);
        Assert.Null(details.Author);
    }

    [Fact]
    public async Task GetByIdAsync_returns_post_details_with_author_when_requested()
    {
        var author = await SeedData.AuthorAsync(_context.DbContext, "Jane", "Smith");
        var post = await SeedData.PostAsync(_context.DbContext, author.Id, "Title");

        var details = await _repository.GetByIdAsync(post.Id, includeAuthor: true);

        Assert.NotNull(details?.Author);
        Assert.Equal(author.Id, details.Author.Id);
        Assert.Equal("Jane", details.Author.Name);
        Assert.Equal("Smith", details.Author.Surname);
    }

    [Fact]
    public async Task GetByIdAsync_omits_removed_author_from_details()
    {
        var author = await SeedData.AuthorAsync(_context.DbContext, removed: true);
        var post = await SeedData.PostAsync(_context.DbContext, author.Id, "Title");

        var details = await _repository.GetByIdAsync(post.Id, includeAuthor: true);

        Assert.NotNull(details);
        Assert.Null(details.Author);
    }

    [Fact]
    public async Task GetByIdAsync_returns_null_for_removed_post()
    {
        var author = await SeedData.AuthorAsync(_context.DbContext);
        var post = await SeedData.PostAsync(_context.DbContext, author.Id, removed: true);

        var details = await _repository.GetByIdAsync(post.Id, includeAuthor: true);

        Assert.Null(details);
    }

    [Fact]
    public async Task GetByIdAsync_returns_null_when_post_does_not_exist()
    {
        var details = await _repository.GetByIdAsync(9999, includeAuthor: true);

        Assert.Null(details);
    }

    public void Dispose() => _context.Dispose();
}
