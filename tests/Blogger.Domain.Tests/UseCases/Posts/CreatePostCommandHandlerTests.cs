using Blogger.Domain.Models.Authors;
using Blogger.Domain.Models.Posts;
using Blogger.Domain.UseCases.Posts.CreatePost;
using FluentValidation;
using Moq;

namespace Blogger.Domain.Tests.UseCases.Posts;

public sealed class CreatePostCommandHandlerTests
{
    private readonly Mock<IAuthorRepository> _authorRepository = new();
    private readonly Mock<IPostRepository> _postRepository = new();
    private readonly CreatePostCommandHandler _handler;

    public CreatePostCommandHandlerTests()
    {
        _handler = new CreatePostCommandHandler(
            _authorRepository.Object,
            _postRepository.Object);
    }

    [Fact]
    public async Task Handle_creates_and_saves_post_when_author_exists()
    {
        var author = Author.Create("John", "Doe");
        var command = new CreatePostCommand(1, "Title", "Description", "Content");

        _authorRepository
            .Setup(repository => repository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        _postRepository
            .Setup(repository => repository.SaveAsync(1, It.IsAny<Post>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int _, Post post, CancellationToken _) => post);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("Title", result.Title);
        Assert.Equal("Description", result.Description);
        Assert.Equal("Content", result.Content);

        _postRepository.Verify(
            repository => repository.SaveAsync(
                1,
                It.Is<Post>(post => post.Title == "Title"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_throws_when_author_does_not_exist()
    {
        var command = new CreatePostCommand(9999, "Title", null, null);

        _authorRepository
            .Setup(repository => repository.GetByIdAsync(9999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => _handler.Handle(command, CancellationToken.None));

        Assert.Contains(
            exception.Errors,
            failure => failure.PropertyName == nameof(command.AuthorId)
                && failure.ErrorMessage == "Author 9999 was not found.");

        _postRepository.Verify(
            repository => repository.SaveAsync(
                It.IsAny<int>(),
                It.IsAny<Post>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
