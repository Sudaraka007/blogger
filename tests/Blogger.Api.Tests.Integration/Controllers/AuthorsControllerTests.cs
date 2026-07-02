using System.Net;
using System.Net.Http.Json;
using Blogger.Api.Contracts.Authors;
using Blogger.Api.Tests.Integration.Infrastructure;
using Blogger.Domain.Models.Authors;
using Blogger.Domain.UseCases.Authors.CreateAuthor;
using Blogger.Domain.UseCases.Authors.GetAuthorById;
using Moq;

namespace Blogger.Api.Tests.Integration.Controllers;

public sealed class AuthorsControllerTests : IClassFixture<BloggerWebApplicationFactory>
{
    private readonly BloggerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AuthorsControllerTests(BloggerWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.MediatorMock.Reset();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_returns_created_author()
    {
        _factory.MediatorMock
            .Setup(m => m.Send(It.IsAny<CreateAuthorCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreateAuthorCommand command, CancellationToken _) =>
                TestData.CreateAuthor(1, command.Name, command.Surname));

        var response = await _client.PostAsJsonAsync(
            "/api/authors",
            new CreateAuthorRequest("John", "Doe"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var author = await response.Content.ReadFromJsonAsync<AuthorResponse>();

        Assert.NotNull(author);
        Assert.Equal(1, author.Id);
        Assert.Equal("John", author.Name);
        Assert.Equal("Doe", author.Surname);
        Assert.Empty(author.Posts);
    }

    [Fact]
    public async Task Create_returns_bad_request_when_body_is_missing()
    {
        using var content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/authors", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        _factory.MediatorMock.Verify(
            m => m.Send(It.IsAny<CreateAuthorCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_returns_bad_request_when_name_is_missing()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/authors",
            new CreateAuthorRequest("", "Doe"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        _factory.MediatorMock.Verify(
            m => m.Send(It.IsAny<CreateAuthorCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetById_returns_author_when_exists()
    {
        _factory.MediatorMock
            .Setup(m => m.Send(It.Is<GetAuthorByIdQuery>(q => q.Id == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.CreateAuthor(1, "Jane", "Smith"));

        var response = await _client.GetAsync("/api/authors/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var author = await response.Content.ReadFromJsonAsync<AuthorResponse>();

        Assert.NotNull(author);
        Assert.Equal(1, author.Id);
        Assert.Equal("Jane", author.Name);
        Assert.Equal("Smith", author.Surname);
    }

    [Fact]
    public async Task GetById_returns_not_found_when_author_does_not_exist()
    {
        _factory.MediatorMock
            .Setup(m => m.Send(It.IsAny<GetAuthorByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        var response = await _client.GetAsync("/api/authors/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
