namespace Blogger.Domain.Authors;

public class Post
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public string? Content { get; set; }

    public bool Removed { get; set; }
}
