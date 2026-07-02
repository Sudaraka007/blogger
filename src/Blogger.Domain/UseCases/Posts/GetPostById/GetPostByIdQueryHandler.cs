using MediatR;

namespace Blogger.Domain.UseCases.Posts.GetPostById;

public sealed class GetPostByIdQueryHandler(IPostDetailsRepository postDetailsRepository)
    : IRequestHandler<GetPostByIdQuery, PostDetails?>
{
    public Task<PostDetails?> Handle(
        GetPostByIdQuery query,
        CancellationToken cancellationToken) =>
        postDetailsRepository.GetByIdAsync(query.Id, query.IncludeAuthor, cancellationToken);
}
