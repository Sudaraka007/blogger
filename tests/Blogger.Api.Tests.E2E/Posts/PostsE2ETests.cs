using System.Net;
using System.Net.Http.Json;
using Blogger.Api.Contracts.Authors;
using Blogger.Api.Contracts.Posts;
using Blogger.Api.Tests.E2E.Infrastructure;

namespace Blogger.Api.Tests.E2E.Posts;

public sealed class PostsE2ETests : IClassFixture<E2eWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PostsE2ETests(E2eWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_returns_created_post()
    {
        var authorId = await CreateAuthorAsync();

        var response = await _client.PostAsJsonAsync(
            "/api/posts",
            new CreatePostRequest(authorId, "Title", "Description", "Content"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var post = await response.Content.ReadFromJsonAsync<PostResponse>();

        Assert.NotNull(post);
        Assert.True(post.Id > 0);
        Assert.Equal("Title", post.Title);
        Assert.Equal("Description", post.Description);
        Assert.Equal("Content", post.Content);
        Assert.Null(post.Author);
    }

    [Fact]
    public async Task Create_returns_bad_request_when_title_is_missing()
    {
        var authorId = await CreateAuthorAsync();

        var response = await _client.PostAsJsonAsync(
            "/api/posts",
            new CreatePostRequest(authorId, "", null, null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_returns_bad_request_when_author_does_not_exist()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/posts",
            new CreatePostRequest(9999, "Title", null, null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_returns_post_when_exists()
    {
        var authorId = await CreateAuthorAsync();

        var createResponse = await _client.PostAsJsonAsync(
            "/api/posts",
            new CreatePostRequest(authorId, "Title", "Description", "Content"));

        var created = await createResponse.Content.ReadFromJsonAsync<PostResponse>();

        var response = await _client.GetAsync($"/api/posts/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var post = await response.Content.ReadFromJsonAsync<PostResponse>();

        Assert.NotNull(post);
        Assert.Equal(created.Id, post.Id);
        Assert.Equal("Title", post.Title);
    }

    [Fact]
    public async Task GetById_returns_post_with_author_when_requested()
    {
        var authorId = await CreateAuthorAsync("John", "Doe");

        var createResponse = await _client.PostAsJsonAsync(
            "/api/posts",
            new CreatePostRequest(authorId, "Title", null, null));

        var created = await createResponse.Content.ReadFromJsonAsync<PostResponse>();

        var response = await _client.GetAsync($"/api/posts/{created!.Id}?includeAuthor=true");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var post = await response.Content.ReadFromJsonAsync<PostResponse>();

        Assert.NotNull(post);
        Assert.NotNull(post.Author);
        Assert.Equal(authorId, post.Author.Id);
        Assert.Equal("John", post.Author.Name);
        Assert.Equal("Doe", post.Author.Surname);
    }

    [Fact]
    public async Task GetById_returns_not_found_when_post_does_not_exist()
    {
        var response = await _client.GetAsync("/api/posts/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<int> CreateAuthorAsync(string name = "John", string surname = "Doe")
    {
        var response = await _client.PostAsJsonAsync(
            "/api/authors",
            new CreateAuthorRequest(name, surname));

        response.EnsureSuccessStatusCode();

        var author = await response.Content.ReadFromJsonAsync<AuthorResponse>();

        return author!.Id;
    }
}
