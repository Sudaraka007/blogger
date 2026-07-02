using System.Reflection;
using Blogger.Domain.Models.Authors;
using Blogger.Domain.Models.Posts;
using Blogger.Domain.UseCases.Posts.GetPostById;

namespace Blogger.Api.Tests.Integration.Infrastructure;

internal static class TestData
{
    public static Author CreateAuthor(
        int id = 1,
        string name = "John",
        string surname = "Doe")
    {
        var author = Author.Create(name, surname);
        SetId(author, id);
        return author;
    }

    public static Post CreatePost(
        int id = 1,
        string title = "Title",
        string? description = "Description",
        string? content = "Content")
    {
        var post = Post.Create(title, description, content);
        SetId(post, id);
        return post;
    }

    public static PostDetails CreatePostDetails(
        int id = 1,
        string title = "Title",
        string? description = "Description",
        string? content = "Content",
        AuthorSummary? author = null) =>
        new(id, title, description, content, author);

    public static AuthorSummary CreateAuthorSummary(
        int id = 1,
        string name = "John",
        string surname = "Doe") =>
        new(id, name, surname);

    private static void SetId<T>(T entity, int id) where T : class =>
        typeof(T).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(entity, id);
}
