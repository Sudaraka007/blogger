using Blogger.Domain.Abstractions;
using Blogger.Domain.Models.Posts;
using MediatR;

namespace Blogger.Domain.UseCases.Posts.CreatePost;

public sealed record CreatePostCommand(
    int AuthorId,
    string Title,
    string? Description,
    string? Content) : IRequest<Post>, IUnitOfWorkCommand, IMonitoredCommand;
