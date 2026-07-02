using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Xml.Linq;
using Blogger.Api.Contracts.Authors;
using Blogger.Api.Tests.E2E.Infrastructure;
using Blogger.Api.XmlContracts.Posts;

namespace Blogger.Api.Tests.E2E.Posts;

public sealed class PostsXmlE2ETests : IClassFixture<E2eWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PostsXmlE2ETests(E2eWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_returns_created_post_xml()
    {
        var authorId = await CreateAuthorAsync();

        using var content = new StringContent(
            CreatePostXml(authorId, "XML Title", "XML Description", "XML Content"),
            Encoding.UTF8,
            "application/xml");

        var response = await _client.PostAsync("/api/xml/posts", content);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("application/xml", response.Content.Headers.ContentType?.MediaType);

        var document = XDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.Root;

        Assert.NotNull(root);
        Assert.Equal("PostResponse", root.Name.LocalName);
        Assert.Equal(XmlPostNamespaces.Contract, root.Name.NamespaceName);
        Assert.True(int.Parse(RequireElement(root, "Id").Value) > 0);
        Assert.Equal("XML Title", RequireElement(root, "Title").Value);
    }

    [Fact]
    public async Task Create_returns_bad_request_when_title_is_missing()
    {
        var authorId = await CreateAuthorAsync();

        using var content = new StringContent(
            CreatePostXml(authorId, "", null, null),
            Encoding.UTF8,
            "application/xml");

        var response = await _client.PostAsync("/api/xml/posts", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_returns_post_xml_with_author_when_requested()
    {
        var authorId = await CreateAuthorAsync("Jane", "Smith");

        using var createContent = new StringContent(
            CreatePostXml(authorId, "XML Title", null, null),
            Encoding.UTF8,
            "application/xml");

        var createResponse = await _client.PostAsync("/api/xml/posts", createContent);
        var created = XDocument.Parse(await createResponse.Content.ReadAsStringAsync());
        var postId = RequireElement(created.Root!, "Id").Value;

        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/xml/posts/{postId}?includeAuthor=true");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var document = XDocument.Parse(await response.Content.ReadAsStringAsync());
        var author = document.Root!.Elements().First(element => element.Name.LocalName == "Author");

        Assert.NotNull(author);
        Assert.Equal(authorId.ToString(), RequireElement(author, "Id").Value);
        Assert.Equal("Jane", RequireElement(author, "Name").Value);
        Assert.Equal("Smith", RequireElement(author, "Surname").Value);
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

    private static string CreatePostXml(int authorId, string title, string? description, string? content)
    {
        var descriptionElement = description is null
            ? string.Empty
            : $"<Description>{description}</Description>";

        var contentElement = content is null
            ? string.Empty
            : $"<Content>{content}</Content>";

        return $"""
                <CreatePostRequest xmlns="{XmlPostNamespaces.Contract}">
                  <AuthorId>{authorId}</AuthorId>
                  <Title>{title}</Title>
                  {descriptionElement}
                  {contentElement}
                </CreatePostRequest>
                """;
    }

    private static XElement RequireElement(XElement parent, string localName) =>
        parent.Element(XName.Get(localName, XmlPostNamespaces.Contract))
        ?? throw new InvalidOperationException($"Expected element '{localName}' was not found.");
}
