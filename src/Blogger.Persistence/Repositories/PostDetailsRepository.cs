using Blogger.Domain.UseCases.Posts.GetPostById;
using Blogger.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using PostEntity = Blogger.Persistence.Entities.Post;

namespace Blogger.Persistence.Repositories;

public class PostDetailsRepository(BloggerDbContext dbContext) : IPostDetailsRepository
{
    public async Task<PostDetails?> GetByIdAsync(
        int id,
        bool includeAuthor,
        CancellationToken cancellationToken = default)
    {
        IQueryable<PostEntity> query = dbContext.Posts.AsNoTracking();

        if (includeAuthor)
        {
            query = query.Include(post => post.Author);
        }

        var entity = await query.FirstOrDefaultAsync(
            post => post.Id == id && !post.Removed,
            cancellationToken);

        if (entity is null)
        {
            return null;
        }

        AuthorSummary? author = null;

        if (includeAuthor && !entity.Author.Removed)
        {
            author = new AuthorSummary(entity.Author.Id, entity.Author.Name, entity.Author.Surname);
        }

        return new PostDetails(
            entity.Id,
            entity.Title,
            entity.Description,
            entity.Content,
            author);
    }
}
