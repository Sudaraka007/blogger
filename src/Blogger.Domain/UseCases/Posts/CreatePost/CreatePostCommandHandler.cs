using Blogger.Domain.Exceptions;
using Blogger.Domain.Models.Authors;
using Blogger.Domain.Models.Posts;
using MediatR;

namespace Blogger.Domain.UseCases.Posts.CreatePost;

public sealed class CreatePostCommandHandler(
    IAuthorRepository authorRepository,
    IPostRepository postRepository)
    : IRequestHandler<CreatePostCommand, Post>
{
    public async Task<Post> Handle(
        CreatePostCommand command,
        CancellationToken cancellationToken)
    {
        var author = await authorRepository.GetByIdAsync(command.AuthorId, cancellationToken);

        if (author is null)
        {
            throw new DomainValidationException(
                nameof(command.AuthorId),
                $"Author {command.AuthorId} was not found.");
        }

        var post = Post.Create(command.Title, command.Description, command.Content);
        return await postRepository.SaveAsync(command.AuthorId, post, cancellationToken);
    }
}
