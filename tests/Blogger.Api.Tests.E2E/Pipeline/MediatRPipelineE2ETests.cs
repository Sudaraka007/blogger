using System.Net;
using System.Net.Http.Json;
using Blogger.Api.Contracts.Authors;
using Blogger.Api.Contracts.Posts;
using Blogger.Api.Tests.E2E.Infrastructure;

namespace Blogger.Api.Tests.E2E.Pipeline;

public sealed class MediatRPipelineE2ETests : IClassFixture<E2eWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MediatRPipelineE2ETests(E2eWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreatePost_runs_domain_validation_through_mediator_pipeline()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/posts",
            new CreatePostRequest(9999, "Title", null, null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("traceId", body, StringComparison.Ordinal);
        Assert.Contains("Author 9999 was not found.", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CreateAuthor_runs_request_validation_through_action_filter()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/authors",
            new CreateAuthorRequest("", ""));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("traceId", body, StringComparison.Ordinal);
    }
}
