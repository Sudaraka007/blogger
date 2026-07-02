using Blogger.Domain.Exceptions;
using Blogger.Domain.Models.Posts;
using Blogger.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using PostEntity = Blogger.Persistence.Entities.Post;

namespace Blogger.Persistence.Repositories;

public class PostRepository(BloggerDbContext dbContext) : IPostRepository
{
    public async Task<Post?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && !p.Removed, cancellationToken);

        return entity is null ? null : PostMapper.ToDomain(entity);
    }

    public async Task<IReadOnlyList<Post>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await dbContext.Posts
            .AsNoTracking()
            .Where(p => !p.Removed)
            .OrderByDescending(p => p.Id)
            .ToListAsync(cancellationToken);

        return entities.Select(PostMapper.ToDomain).ToList();
    }

    public async Task<IReadOnlyList<Post>> GetByAuthorIdAsync(int authorId, CancellationToken cancellationToken = default)
    {
        var entities = await dbContext.Posts
            .AsNoTracking()
            .Where(p => p.AuthorId == authorId && !p.Removed)
            .OrderByDescending(p => p.Id)
            .ToListAsync(cancellationToken);

        return entities.Select(PostMapper.ToDomain).ToList();
    }

    public async Task<Post> SaveAsync(int authorId, Post post, CancellationToken cancellationToken = default)
    {
        if (post.Id == 0)
        {
            var authorIsActive = await dbContext.Authors
                .AsNoTracking()
                .AnyAsync(a => a.Id == authorId && !a.Removed, cancellationToken);

            if (!authorIsActive)
            {
                throw new DomainValidationException(
                    "AuthorId",
                    $"Author {authorId} was not found.");
            }

            var entity = new PostEntity
            {
                AuthorId = authorId,
                Title = post.Title,
                Description = post.Description,
                Content = post.Content,
                Removed = post.Removed
            };

            dbContext.Posts.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
            return PostMapper.ToDomain(entity);
        }

        var existing = await dbContext.Posts
            .FirstOrDefaultAsync(p => p.Id == post.Id && p.AuthorId == authorId, cancellationToken)
            ?? throw new InvalidOperationException($"Post {post.Id} was not found for author {authorId}.");

        existing.Title = post.Title;
        existing.Description = post.Description;
        existing.Content = post.Content;
        existing.Removed = post.Removed;

        await dbContext.SaveChangesAsync(cancellationToken);
        return PostMapper.ToDomain(existing);
    }
}
