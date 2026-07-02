namespace Blogger.Domain.UseCases.Posts.GetPostById;

public interface IPostDetailsRepository
{
    Task<PostDetails?> GetByIdAsync(
        int id,
        bool includeAuthor,
        CancellationToken cancellationToken = default);
}
