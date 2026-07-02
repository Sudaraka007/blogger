namespace Blogger.Domain.Models.Posts;

public class Post
{
    public int Id { get; private set; }

    public string Title { get; private set; } = null!;

    public string? Description { get; private set; }

    public string? Content { get; private set; }

    public bool Removed { get; private set; }

    public static Post Create(string title, string? description, string? content)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        return new Post
        {
            Title = title.Trim(),
            Description = description?.Trim(),
            Content = content?.Trim()
        };
    }

    internal static Post Reconstitute(
        int id,
        string title,
        string? description,
        string? content,
        bool removed)
    {
        return new Post
        {
            Id = id,
            Title = title,
            Description = description,
            Content = content,
            Removed = removed
        };
    }

    public void Update(string title, string? description, string? content)
    {
        if (Removed)
        {
            throw new InvalidOperationException("Cannot update a removed post.");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        Title = title.Trim();
        Description = description?.Trim();
        Content = content?.Trim();
    }

    public void Remove()
    {
        Removed = true;
    }
}
