namespace Blogger.Domain.UseCases.Posts.GetPostById;

public sealed record PostDetails(
    int Id,
    string Title,
    string? Description,
    string? Content,
    AuthorSummary? Author);
