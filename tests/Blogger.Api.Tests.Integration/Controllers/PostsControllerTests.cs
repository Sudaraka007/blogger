using System.Net;
using System.Net.Http.Json;
using Blogger.Api.Contracts.Posts;
using Blogger.Api.Tests.Integration.Infrastructure;
using Blogger.Domain.UseCases.Posts.CreatePost;
using Blogger.Domain.UseCases.Posts.GetPostById;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Blogger.Api.Tests.Integration.Controllers;

public sealed class PostsControllerTests : IClassFixture<BloggerWebApplicationFactory>
{
    private readonly BloggerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public PostsControllerTests(BloggerWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.MediatorMock.Reset();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_returns_created_post()
    {
        _factory.MediatorMock
            .Setup(m => m.Send(It.IsAny<CreatePostCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.CreatePost(1, "Title", "Description", "Content"));

        var response = await _client.PostAsJsonAsync(
            "/api/posts",
            new CreatePostRequest(1, "Title", "Description", "Content"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var post = await response.Content.ReadFromJsonAsync<PostResponse>();

        Assert.NotNull(post);
        Assert.Equal(1, post.Id);
        Assert.Equal("Title", post.Title);
        Assert.Equal("Description", post.Description);
        Assert.Equal("Content", post.Content);
        Assert.Null(post.Author);
    }

    [Fact]
    public async Task Create_returns_bad_request_when_body_is_missing()
    {
        using var content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/posts", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        _factory.MediatorMock.Verify(
            m => m.Send(It.IsAny<CreatePostCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_returns_bad_request_when_title_is_missing()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/posts",
            new CreatePostRequest(1, "", null, null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        _factory.MediatorMock.Verify(
            m => m.Send(It.IsAny<CreatePostCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_returns_bad_request_when_author_does_not_exist()
    {
        _factory.MediatorMock
            .Setup(m => m.Send(It.IsAny<CreatePostCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException([
                new ValidationFailure("AuthorId", "Author 9999 was not found.")
            ]));

        var response = await _client.PostAsJsonAsync(
            "/api/posts",
            new CreatePostRequest(9999, "Title", null, null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_returns_post_when_exists()
    {
        _factory.MediatorMock
            .Setup(m => m.Send(It.Is<GetPostByIdQuery>(q => q.Id == 1 && !q.IncludeAuthor), It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.CreatePostDetails(1, "Title", "Description", "Content"));

        var response = await _client.GetAsync("/api/posts/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var post = await response.Content.ReadFromJsonAsync<PostResponse>();

        Assert.NotNull(post);
        Assert.Equal(1, post.Id);
        Assert.Equal("Title", post.Title);
    }

    [Fact]
    public async Task GetById_returns_post_with_author_when_requested()
    {
        _factory.MediatorMock
            .Setup(m => m.Send(It.Is<GetPostByIdQuery>(q => q.Id == 1 && q.IncludeAuthor), It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.CreatePostDetails(
                1,
                "Title",
                author: TestData.CreateAuthorSummary(1, "John", "Doe")));

        var response = await _client.GetAsync("/api/posts/1?includeAuthor=true");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var post = await response.Content.ReadFromJsonAsync<PostResponse>();

        Assert.NotNull(post);
        Assert.NotNull(post.Author);
        Assert.Equal(1, post.Author.Id);
        Assert.Equal("John", post.Author.Name);
        Assert.Equal("Doe", post.Author.Surname);
    }

    [Fact]
    public async Task GetById_returns_bad_request_when_includeAuthor_is_invalid()
    {
        var response = await _client.GetAsync("/api/posts/1?includeAuthor=not-a-bool");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        _factory.MediatorMock.Verify(
            m => m.Send(It.IsAny<GetPostByIdQuery>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetById_returns_not_found_when_post_does_not_exist()
    {
        _factory.MediatorMock
            .Setup(m => m.Send(It.IsAny<GetPostByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PostDetails?)null);

        var response = await _client.GetAsync("/api/posts/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
