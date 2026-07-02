namespace Blogger.Api.Contracts.Posts;

public sealed record CreatePostRequest(int AuthorId, string Title, string? Description, string? Content);
