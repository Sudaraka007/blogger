using System.Reflection;
using Blogger.Domain.Models.Authors;
using Blogger.Domain.Models.Posts;

namespace Blogger.Persistence.Tests.Infrastructure;

internal static class DomainTestHelpers
{
    public static Author AuthorWithId(
        int id,
        string name = "John",
        string surname = "Doe",
        bool removed = false)
    {
        var author = Author.Create(name, surname);

        if (removed)
        {
            author.Remove();
        }

        SetId(author, id);
        return author;
    }

    public static Post PostWithId(
        int id,
        string title = "Title",
        string? description = "Description",
        string? content = "Content",
        bool removed = false)
    {
        var post = Post.Create(title, description, content);

        if (removed)
        {
            post.Remove();
        }

        SetId(post, id);
        return post;
    }

    private static void SetId<T>(T entity, int id) where T : class =>
        typeof(T).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(entity, id);
}
