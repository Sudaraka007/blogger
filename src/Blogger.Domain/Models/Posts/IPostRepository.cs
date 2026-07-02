namespace Blogger.Domain.Models.Posts;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Post>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Post>> GetByAuthorIdAsync(int authorId, CancellationToken cancellationToken = default);

    Task<Post> SaveAsync(int authorId, Post post, CancellationToken cancellationToken = default);
}
