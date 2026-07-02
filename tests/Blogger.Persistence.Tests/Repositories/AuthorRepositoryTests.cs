using Blogger.Domain.Models.Authors;
using Blogger.Persistence.Repositories;
using Blogger.Persistence.Tests.Infrastructure;

namespace Blogger.Persistence.Tests.Repositories;

public sealed class AuthorRepositoryTests : IDisposable
{
    private readonly PersistenceTestContext _context = new();
    private readonly AuthorRepository _repository;

    public AuthorRepositoryTests()
    {
        _repository = new AuthorRepository(_context.DbContext);
    }

    [Fact]
    public async Task SaveAsync_creates_author_with_generated_id()
    {
        var author = Author.Create("John", "Doe");

        var saved = await _repository.SaveAsync(author);

        Assert.True(saved.Id > 0);
        Assert.Equal("John", saved.Name);
        Assert.Equal("Doe", saved.Surname);
    }

    [Fact]
    public async Task SaveAsync_updates_existing_author()
    {
        var saved = await _repository.SaveAsync(Author.Create("John", "Doe"));
        saved.Rename("Jane", "Smith");

        var updated = await _repository.SaveAsync(saved);

        Assert.Equal(saved.Id, updated.Id);
        Assert.Equal("Jane", updated.Name);
        Assert.Equal("Smith", updated.Surname);
    }

    [Fact]
    public async Task SaveAsync_throws_when_updating_missing_author()
    {
        var author = DomainTestHelpers.AuthorWithId(9999);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _repository.SaveAsync(author));
    }

    [Fact]
    public async Task GetByIdAsync_returns_author_with_non_removed_posts()
    {
        var authorEntity = await SeedData.AuthorAsync(_context.DbContext);
        await SeedData.PostAsync(_context.DbContext, authorEntity.Id, "Active");
        await SeedData.PostAsync(_context.DbContext, authorEntity.Id, "Removed", removed: true);

        var author = await _repository.GetByIdAsync(authorEntity.Id);

        Assert.NotNull(author);
        Assert.Single(author.Posts);
        Assert.Equal("Active", author.Posts[0].Title);
    }

    [Fact]
    public async Task GetByIdAsync_returns_null_for_removed_author()
    {
        var authorEntity = await SeedData.AuthorAsync(_context.DbContext, removed: true);

        var author = await _repository.GetByIdAsync(authorEntity.Id);

        Assert.Null(author);
    }

    [Fact]
    public async Task GetAllAsync_returns_authors_ordered_by_surname_then_name()
    {
        await SeedData.AuthorAsync(_context.DbContext, "John", "Smith");
        await SeedData.AuthorAsync(_context.DbContext, "Jane", "Adams");
        await SeedData.AuthorAsync(_context.DbContext, "Jack", "Adams");

        var authors = await _repository.GetAllAsync();

        Assert.Equal(3, authors.Count);
        Assert.Equal("Adams", authors[0].Surname);
        Assert.Equal("Jack", authors[0].Name);
        Assert.Equal("Adams", authors[1].Surname);
        Assert.Equal("Jane", authors[1].Name);
        Assert.Equal("Smith", authors[2].Surname);
    }

    [Fact]
    public async Task GetAllAsync_excludes_removed_authors()
    {
        await SeedData.AuthorAsync(_context.DbContext, "John", "Doe");
        await SeedData.AuthorAsync(_context.DbContext, "Jane", "Smith", removed: true);

        var authors = await _repository.GetAllAsync();

        Assert.Single(authors);
        Assert.Equal("John", authors[0].Name);
    }

    public void Dispose() => _context.Dispose();
}
