namespace Blogger.Domain.Authors;

public class Author
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Surname { get; set; }

    public bool Removed { get; set; }

    public IList<Post> Posts { get; set; } = [];
}
