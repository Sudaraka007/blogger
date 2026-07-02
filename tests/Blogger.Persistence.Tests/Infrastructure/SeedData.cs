using Blogger.Persistence.Data;
using Blogger.Persistence.Entities;

namespace Blogger.Persistence.Tests.Infrastructure;

internal static class SeedData
{
    public static async Task<Author> AuthorAsync(
        BloggerDbContext dbContext,
        string name = "John",
        string surname = "Doe",
        bool removed = false)
    {
        var entity = new Author
        {
            Name = name,
            Surname = surname,
            Removed = removed
        };

        dbContext.Authors.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }

    public static async Task<Post> PostAsync(
        BloggerDbContext dbContext,
        int authorId,
        string title = "Title",
        string? description = "Description",
        string? content = "Content",
        bool removed = false)
    {
        var entity = new Post
        {
            AuthorId = authorId,
            Title = title,
            Description = description,
            Content = content,
            Removed = removed
        };

        dbContext.Posts.Add(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }
}
