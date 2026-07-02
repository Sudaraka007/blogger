using Blogger.Domain.UseCases.Posts.GetPostById;
using Moq;

namespace Blogger.Domain.Tests.UseCases.Posts;

public sealed class GetPostByIdQueryHandlerTests
{
    private readonly Mock<IPostDetailsRepository> _postDetailsRepository = new();
    private readonly GetPostByIdQueryHandler _handler;

    public GetPostByIdQueryHandlerTests()
    {
        _handler = new GetPostByIdQueryHandler(_postDetailsRepository.Object);
    }

    [Fact]
    public async Task Handle_returns_post_details_from_repository()
    {
        var details = new PostDetails(1, "Title", "Description", "Content", null);
        var query = new GetPostByIdQuery(1);

        _postDetailsRepository
            .Setup(repository => repository.GetByIdAsync(1, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(details);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Same(details, result);
    }

    [Fact]
    public async Task Handle_passes_include_author_flag_to_repository()
    {
        var details = new PostDetails(
            1,
            "Title",
            null,
            null,
            new AuthorSummary(2, "Jane", "Smith"));
        var query = new GetPostByIdQuery(1, IncludeAuthor: true);

        _postDetailsRepository
            .Setup(repository => repository.GetByIdAsync(1, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(details);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result?.Author);
        Assert.Equal("Jane", result.Author.Name);
    }

    [Fact]
    public async Task Handle_returns_null_when_post_does_not_exist()
    {
        var query = new GetPostByIdQuery(9999);

        _postDetailsRepository
            .Setup(repository => repository.GetByIdAsync(9999, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PostDetails?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }
}
