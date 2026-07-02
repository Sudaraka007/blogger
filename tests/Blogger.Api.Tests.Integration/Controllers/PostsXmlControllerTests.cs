using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using Blogger.Api.Tests.Integration.Infrastructure;
using Blogger.Api.XmlContracts.Posts;
using Blogger.Domain.UseCases.Posts.CreatePost;
using Blogger.Domain.UseCases.Posts.GetPostById;
using Moq;

namespace Blogger.Api.Tests.Integration.Controllers;

public sealed class PostsXmlControllerTests : IClassFixture<BloggerWebApplicationFactory>
{
    private readonly BloggerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public PostsXmlControllerTests(BloggerWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.MediatorMock.Reset();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_returns_created_post_xml()
    {
        _factory.MediatorMock
            .Setup(m => m.Send(It.IsAny<CreatePostCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.CreatePost(1, "XML Title", "XML Description", "XML Content"));

        using var content = new StringContent(
            CreatePostXml(1, "XML Title", "XML Description", "XML Content"),
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
        Assert.Equal("1", RequireElement(root, "Id").Value);
        Assert.Equal("XML Title", RequireElement(root, "Title").Value);
    }

    [Fact]
    public async Task Create_returns_bad_request_when_title_is_missing()
    {
        using var content = new StringContent(
            CreatePostXml(1, "", null, null),
            Encoding.UTF8,
            "application/xml");

        var response = await _client.PostAsync("/api/xml/posts", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        _factory.MediatorMock.Verify(
            m => m.Send(It.IsAny<CreatePostCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetById_returns_post_xml_with_author_when_requested()
    {
        _factory.MediatorMock
            .Setup(m => m.Send(It.Is<GetPostByIdQuery>(q => q.Id == 1 && q.IncludeAuthor), It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.CreatePostDetails(
                1,
                "XML Title",
                author: TestData.CreateAuthorSummary(2, "Jane", "Smith")));

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/xml/posts/1?includeAuthor=true");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var document = XDocument.Parse(await response.Content.ReadAsStringAsync());
        var author = document.Root!.Elements().First(element => element.Name.LocalName == "Author");

        Assert.NotNull(author);
        Assert.Equal("2", RequireElement(author, "Id").Value);
        Assert.Equal("Jane", RequireElement(author, "Name").Value);
        Assert.Equal("Smith", RequireElement(author, "Surname").Value);
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
