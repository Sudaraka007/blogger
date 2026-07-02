using Blogger.Domain.Models.Authors;
using Blogger.Domain.UseCases.Authors.GetAuthorById;
using Moq;

namespace Blogger.Domain.Tests.UseCases.Authors;

public sealed class GetAuthorByIdQueryHandlerTests
{
    private readonly Mock<IAuthorRepository> _authorRepository = new();
    private readonly GetAuthorByIdQueryHandler _handler;

    public GetAuthorByIdQueryHandlerTests()
    {
        _handler = new GetAuthorByIdQueryHandler(_authorRepository.Object);
    }

    [Fact]
    public async Task Handle_returns_author_from_repository()
    {
        var author = Author.Create("Jane", "Smith");
        var query = new GetAuthorByIdQuery(1);

        _authorRepository
            .Setup(repository => repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Same(author, result);
    }

    [Fact]
    public async Task Handle_returns_null_when_author_does_not_exist()
    {
        var query = new GetAuthorByIdQuery(9999);

        _authorRepository
            .Setup(repository => repository.GetByIdAsync(9999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }
}
