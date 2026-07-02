using Blogger.Domain.Models.Authors;
using Blogger.Domain.Models.Posts;

namespace Blogger.Domain.Tests.Models.Authors;

public sealed class AuthorTests
{
    [Fact]
    public void Create_sets_name_and_surname()
    {
        var author = Author.Create("  John  ", "  Doe  ");

        Assert.Equal(0, author.Id);
        Assert.Equal("John", author.Name);
        Assert.Equal("Doe", author.Surname);
        Assert.False(author.Removed);
        Assert.Empty(author.Posts);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_throws_when_name_is_missing(string? name)
    {
        var exception = Assert.Throws<ArgumentException>(() => Author.Create(name!, "Doe"));

        Assert.Equal("name", exception.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_throws_when_surname_is_missing(string? surname)
    {
        var exception = Assert.Throws<ArgumentException>(() => Author.Create("John", surname!));

        Assert.Equal("surname", exception.ParamName);
    }

    [Fact]
    public void Rename_updates_name_and_surname()
    {
        var author = Author.Create("John", "Doe");

        author.Rename("  Jane  ", "  Smith  ");

        Assert.Equal("Jane", author.Name);
        Assert.Equal("Smith", author.Surname);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Rename_throws_when_name_is_missing(string? name)
    {
        var author = Author.Create("John", "Doe");

        var exception = Assert.Throws<ArgumentException>(() => author.Rename(name!, "Smith"));

        Assert.Equal("name", exception.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Rename_throws_when_surname_is_missing(string? surname)
    {
        var author = Author.Create("John", "Doe");

        var exception = Assert.Throws<ArgumentException>(() => author.Rename("Jane", surname!));

        Assert.Equal("surname", exception.ParamName);
    }

    [Fact]
    public void Rename_throws_when_author_is_removed()
    {
        var author = Author.Create("John", "Doe");
        author.Remove();

        Assert.Throws<InvalidOperationException>(() => author.Rename("Jane", "Smith"));
    }

    [Fact]
    public void AddPost_adds_post_to_author()
    {
        var author = Author.Create("John", "Doe");

        var post = author.AddPost("  Title  ", "  Description  ", "  Content  ");

        Assert.Single(author.Posts);
        Assert.Same(post, author.Posts[0]);
        Assert.Equal("Title", post.Title);
        Assert.Equal("Description", post.Description);
        Assert.Equal("Content", post.Content);
    }

    [Fact]
    public void AddPost_allows_null_description_and_content()
    {
        var author = Author.Create("John", "Doe");

        var post = author.AddPost("Title", null, null);

        Assert.Null(post.Description);
        Assert.Null(post.Content);
    }

    [Fact]
    public void AddPost_adds_multiple_posts()
    {
        var author = Author.Create("John", "Doe");

        author.AddPost("First", null, null);
        author.AddPost("Second", null, null);

        Assert.Equal(2, author.Posts.Count);
        Assert.Equal("First", author.Posts[0].Title);
        Assert.Equal("Second", author.Posts[1].Title);
    }

    [Fact]
    public void AddPost_throws_when_author_is_removed()
    {
        var author = Author.Create("John", "Doe");
        author.Remove();

        Assert.Throws<InvalidOperationException>(() => author.AddPost("Title", null, null));
    }

    [Fact]
    public void Remove_marks_author_as_removed()
    {
        var author = Author.Create("John", "Doe");

        author.Remove();

        Assert.True(author.Removed);
    }
}
