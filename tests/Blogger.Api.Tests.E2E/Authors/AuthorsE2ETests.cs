using System.Net;
using System.Net.Http.Json;
using Blogger.Api.Contracts.Authors;
using Blogger.Api.Tests.E2E.Infrastructure;

namespace Blogger.Api.Tests.E2E.Authors;

public sealed class AuthorsE2ETests : IClassFixture<E2eWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthorsE2ETests(E2eWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_returns_created_author()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/authors",
            new CreateAuthorRequest("John", "Doe"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var author = await response.Content.ReadFromJsonAsync<AuthorResponse>();

        Assert.NotNull(author);
        Assert.True(author.Id > 0);
        Assert.Equal("John", author.Name);
        Assert.Equal("Doe", author.Surname);
        Assert.Empty(author.Posts);
    }

    [Fact]
    public async Task Create_returns_bad_request_when_name_is_missing()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/authors",
            new CreateAuthorRequest("", "Doe"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_returns_author_when_exists()
    {
        var createResponse = await _client.PostAsJsonAsync(
            "/api/authors",
            new CreateAuthorRequest("Jane", "Smith"));

        var created = await createResponse.Content.ReadFromJsonAsync<AuthorResponse>();

        var response = await _client.GetAsync($"/api/authors/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var author = await response.Content.ReadFromJsonAsync<AuthorResponse>();

        Assert.NotNull(author);
        Assert.Equal(created.Id, author.Id);
        Assert.Equal("Jane", author.Name);
        Assert.Equal("Smith", author.Surname);
    }

    [Fact]
    public async Task GetById_returns_not_found_when_author_does_not_exist()
    {
        var response = await _client.GetAsync("/api/authors/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
