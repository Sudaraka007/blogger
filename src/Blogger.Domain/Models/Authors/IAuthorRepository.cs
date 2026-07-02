namespace Blogger.Domain.Models.Authors;

public interface IAuthorRepository
{
    Task<Author?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Author>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Author> SaveAsync(Author author, CancellationToken cancellationToken = default);
}
