namespace Blogger.Domain.Models.Authors;

using Blogger.Domain.Models.Posts;

public class Author
{
    private readonly List<Post> _posts = [];

    public int Id { get; private set; }

    public string Name { get; private set; } = null!;

    public string Surname { get; private set; } = null!;

    public bool Removed { get; private set; }

    public IReadOnlyList<Post> Posts => _posts;

    public static Author Create(string name, string surname)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(surname))
        {
            throw new ArgumentException("Surname is required.", nameof(surname));
        }

        return new Author
        {
            Name = name.Trim(),
            Surname = surname.Trim()
        };
    }

    internal static Author Reconstitute(
        int id,
        string name,
        string surname,
        bool removed,
        IEnumerable<Post> posts)
    {
        var author = new Author
        {
            Id = id,
            Name = name,
            Surname = surname,
            Removed = removed
        };

        author._posts.AddRange(posts);
        return author;
    }

    public void Rename(string name, string surname)
    {
        if (Removed)
        {
            throw new InvalidOperationException("Cannot rename a removed author.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(surname))
        {
            throw new ArgumentException("Surname is required.", nameof(surname));
        }

        Name = name.Trim();
        Surname = surname.Trim();
    }

    public Post AddPost(string title, string? description, string? content)
    {
        if (Removed)
        {
            throw new InvalidOperationException("Cannot add posts to a removed author.");
        }

        var post = Post.Create(title, description, content);
        _posts.Add(post);
        return post;
    }

    public void Remove()
    {
        Removed = true;
    }
}
