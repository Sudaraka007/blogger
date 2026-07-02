using Blogger.Domain.Abstractions;
using MediatR;

namespace Blogger.Domain.UseCases.Posts.GetPostById;

public sealed record GetPostByIdQuery(int Id, bool IncludeAuthor = false)
    : IRequest<PostDetails?>, IMonitoredCommand;
