using Blogger.Domain.Models.Authors;

namespace Blogger.Api.Contracts.Authors;

public sealed record AuthorPostResponse(int Id, string Title, string? Description);

public sealed record AuthorResponse(int Id, string Name, string Surname, IReadOnlyList<AuthorPostResponse> Posts)
{
    public static AuthorResponse FromDomain(Author author) =>
        new(
            author.Id,
            author.Name,
            author.Surname,
            author.Posts
                .Select(post => new AuthorPostResponse(post.Id, post.Title, post.Description))
                .ToList());
}
