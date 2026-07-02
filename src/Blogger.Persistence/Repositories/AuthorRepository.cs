using Blogger.Domain.Authors;
using Blogger.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using AuthorEntity = Blogger.Persistence.Entities.Author;
using PostEntity = Blogger.Persistence.Entities.Post;

namespace Blogger.Persistence.Repositories;

public class AuthorRepository(BloggerDbContext dbContext) : IAuthorRepository
{
    public async Task<Author?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Authors
            .AsNoTracking()
            .Include(a => a.Posts.Where(p => !p.Removed))
            .FirstOrDefaultAsync(a => a.Id == id && !a.Removed, cancellationToken);

        return entity is null ? null : ToDomain(entity, includePosts: true);
    }

    public async Task<IReadOnlyList<Author>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await dbContext.Authors
            .AsNoTracking()
            .Where(a => !a.Removed)
            .OrderBy(a => a.Surname)
            .ThenBy(a => a.Name)
            .ToListAsync(cancellationToken);

        return entities.Select(e => ToDomain(e)).ToList();
    }

    public async Task<Author> SaveAsync(Author author, CancellationToken cancellationToken = default)
    {
        if (author.Id == 0)
        {
            var entity = new AuthorEntity
            {
                Name = author.Name,
                Surname = author.Surname,
                Removed = author.Removed
            };

            dbContext.Authors.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
            return ToDomain(entity);
        }

        var existing = await dbContext.Authors
            .FirstOrDefaultAsync(a => a.Id == author.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Author {author.Id} was not found.");

        existing.Name = author.Name;
        existing.Surname = author.Surname;
        existing.Removed = author.Removed;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDomain(existing);
    }

    private static Author ToDomain(AuthorEntity entity, bool includePosts = false) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Surname = entity.Surname,
        Removed = entity.Removed,
        Posts = includePosts
            ? entity.Posts.Select(PostMapper.ToDomain).ToList()
            : []
    };
}

internal static class PostMapper
{
    public static Post ToDomain(PostEntity entity) => new()
    {
        Id = entity.Id,
        Title = entity.Title,
        Description = entity.Description,
        Content = entity.Content,
        Removed = entity.Removed
    };
}
