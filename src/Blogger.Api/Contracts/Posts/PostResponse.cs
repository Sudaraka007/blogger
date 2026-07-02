using Blogger.Domain.Models.Posts;
using Blogger.Domain.UseCases.Posts.GetPostById;

namespace Blogger.Api.Contracts.Posts;

public sealed record PostAuthorResponse(int Id, string Name, string Surname);

public sealed record PostResponse(
    int Id,
    string Title,
    string? Description,
    string? Content,
    PostAuthorResponse? Author = null)
{
    public static PostResponse FromDomain(Post post) =>
        new(post.Id, post.Title, post.Description, post.Content);

    public static PostResponse FromDetails(PostDetails details) =>
        new(
            details.Id,
            details.Title,
            details.Description,
            details.Content,
            details.Author is null
                ? null
                : new PostAuthorResponse(details.Author.Id, details.Author.Name, details.Author.Surname));
}
