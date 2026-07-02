using Blogger.Domain.Abstractions;
using Blogger.Domain.Models.Posts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Blogger.Domain.UseCases.Posts.CreatePost;

public sealed record CreatePostCommand(
    int AuthorId,
    string Title,
    string? Description,
    string? Content) : IRequest<Post>, IUnitOfWorkCommand, IMonitoredCommand
{
    public void LogSuccess(ILogger logger) =>
        logger.LogInformation(
            "CreatePostCommand succeeded for AuthorId {AuthorId} with title {Title}",
            AuthorId,
            Title);

    public void LogFailure(ILogger logger, Exception exception) =>
        logger.LogError(
            exception,
            "CreatePostCommand failed for AuthorId {AuthorId} with title {Title}",
            AuthorId,
            Title);
}
