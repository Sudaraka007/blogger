using Blogger.Domain.Models.Posts;
using Blogger.Persistence.Repositories;
using Blogger.Persistence.Tests.Infrastructure;

namespace Blogger.Persistence.Tests.Repositories;

public sealed class PostRepositoryTests : IDisposable
{
    private readonly PersistenceTestContext _context = new();
    private readonly PostRepository _repository;

    public PostRepositoryTests()
    {
        _repository = new PostRepository(_context.DbContext);
    }

    [Fact]
    public async Task SaveAsync_creates_post_with_generated_id()
    {
        var author = await SeedData.AuthorAsync(_context.DbContext);
        var post = Post.Create("Title", "Description", "Content");

        var saved = await _repository.SaveAsync(author.Id, post);

        Assert.True(saved.Id > 0);
        Assert.Equal("Title", saved.Title);
    }

    [Fact]
    public async Task SaveAsync_updates_existing_post()
    {
        var author = await SeedData.AuthorAsync(_context.DbContext);
        var saved = await _repository.SaveAsync(author.Id, Post.Create("Title", null, null));
        saved.Update("Updated", "Description", "Content");

        var updated = await _repository.SaveAsync(author.Id, saved);

        Assert.Equal(saved.Id, updated.Id);
        Assert.Equal("Updated", updated.Title);
        Assert.Equal("Description", updated.Description);
        Assert.Equal("Content", updated.Content);
    }

    [Fact]
    public async Task SaveAsync_throws_when_updating_missing_post()
    {
        var author = await SeedData.AuthorAsync(_context.DbContext);
        var post = DomainTestHelpers.PostWithId(9999);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _repository.SaveAsync(author.Id, post));
    }

    [Fact]
    public async Task GetByIdAsync_returns_post_when_exists()
    {
        var author = await SeedData.AuthorAsync(_context.DbContext);
        var postEntity = await SeedData.PostAsync(_context.DbContext, author.Id, "Title");

        var post = await _repository.GetByIdAsync(postEntity.Id);

        Assert.NotNull(post);
        Assert.Equal("Title", post.Title);
    }

    [Fact]
    public async Task GetByIdAsync_returns_null_for_removed_post()
    {
        var author = await SeedData.AuthorAsync(_context.DbContext);
        var postEntity = await SeedData.PostAsync(_context.DbContext, author.Id, removed: true);

        var post = await _repository.GetByIdAsync(postEntity.Id);

        Assert.Null(post);
    }

    [Fact]
    public async Task GetAllAsync_returns_posts_ordered_by_id_descending()
    {
        var author = await SeedData.AuthorAsync(_context.DbContext);
        var first = await SeedData.PostAsync(_context.DbContext, author.Id, "First");
        var second = await SeedData.PostAsync(_context.DbContext, author.Id, "Second");

        var posts = await _repository.GetAllAsync();

        Assert.Equal(2, posts.Count);
        Assert.Equal(second.Id, posts[0].Id);
        Assert.Equal(first.Id, posts[1].Id);
    }

    [Fact]
    public async Task GetByAuthorIdAsync_returns_only_posts_for_author()
    {
        var firstAuthor = await SeedData.AuthorAsync(_context.DbContext, "John", "Doe");
        var secondAuthor = await SeedData.AuthorAsync(_context.DbContext, "Jane", "Smith");
        var post = await SeedData.PostAsync(_context.DbContext, firstAuthor.Id, "Author Post");
        await SeedData.PostAsync(_context.DbContext, secondAuthor.Id, "Other Post");

        var posts = await _repository.GetByAuthorIdAsync(firstAuthor.Id);

        Assert.Single(posts);
        Assert.Equal(post.Id, posts[0].Id);
    }

    [Fact]
    public async Task GetByAuthorIdAsync_excludes_removed_posts()
    {
        var author = await SeedData.AuthorAsync(_context.DbContext);
        await SeedData.PostAsync(_context.DbContext, author.Id, "Active");
        await SeedData.PostAsync(_context.DbContext, author.Id, "Removed", removed: true);

        var posts = await _repository.GetByAuthorIdAsync(author.Id);

        Assert.Single(posts);
        Assert.Equal("Active", posts[0].Title);
    }

    public void Dispose() => _context.Dispose();
}
