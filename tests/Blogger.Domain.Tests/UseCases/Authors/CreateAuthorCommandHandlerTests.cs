using Blogger.Domain.Models.Authors;
using Blogger.Domain.UseCases.Authors.CreateAuthor;
using Moq;

namespace Blogger.Domain.Tests.UseCases.Authors;

public sealed class CreateAuthorCommandHandlerTests
{
    private readonly Mock<IAuthorRepository> _authorRepository = new();
    private readonly CreateAuthorCommandHandler _handler;

    public CreateAuthorCommandHandlerTests()
    {
        _handler = new CreateAuthorCommandHandler(_authorRepository.Object);
    }

    [Fact]
    public async Task Handle_creates_and_saves_author()
    {
        var command = new CreateAuthorCommand("John", "Doe");

        _authorRepository
            .Setup(repository => repository.SaveAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author author, CancellationToken _) => author);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("John", result.Name);
        Assert.Equal("Doe", result.Surname);

        _authorRepository.Verify(
            repository => repository.SaveAsync(
                It.Is<Author>(author => author.Name == "John" && author.Surname == "Doe"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
