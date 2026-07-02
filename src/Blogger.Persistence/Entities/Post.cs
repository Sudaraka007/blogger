namespace Blogger.Persistence.Entities;

public class Post
{
    public int Id { get; set; }

    public int AuthorId { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public string? Content { get; set; }

    public bool Removed { get; set; }

    public Author Author { get; set; } = null!;
}
