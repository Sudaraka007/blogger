using Blogger.Domain.Models.Posts;

namespace Blogger.Domain.Tests.Models.Posts;

public sealed class PostTests
{
    [Fact]
    public void Create_sets_properties()
    {
        var post = Post.Create("  Title  ", "  Description  ", "  Content  ");

        Assert.Equal(0, post.Id);
        Assert.Equal("Title", post.Title);
        Assert.Equal("Description", post.Description);
        Assert.Equal("Content", post.Content);
        Assert.False(post.Removed);
    }

    [Fact]
    public void Create_allows_null_description_and_content()
    {
        var post = Post.Create("Title", null, null);

        Assert.Null(post.Description);
        Assert.Null(post.Content);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_throws_when_title_is_missing(string? title)
    {
        var exception = Assert.Throws<ArgumentException>(() => Post.Create(title!, null, null));

        Assert.Equal("title", exception.ParamName);
    }

    [Fact]
    public void Update_updates_properties()
    {
        var post = Post.Create("Title", "Description", "Content");

        post.Update("  New Title  ", "  New Description  ", "  New Content  ");

        Assert.Equal("New Title", post.Title);
        Assert.Equal("New Description", post.Description);
        Assert.Equal("New Content", post.Content);
    }

    [Fact]
    public void Update_allows_null_description_and_content()
    {
        var post = Post.Create("Title", "Description", "Content");

        post.Update("New Title", null, null);

        Assert.Null(post.Description);
        Assert.Null(post.Content);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_throws_when_title_is_missing(string? title)
    {
        var post = Post.Create("Title", null, null);

        var exception = Assert.Throws<ArgumentException>(() => post.Update(title!, null, null));

        Assert.Equal("title", exception.ParamName);
    }

    [Fact]
    public void Update_throws_when_post_is_removed()
    {
        var post = Post.Create("Title", null, null);
        post.Remove();

        Assert.Throws<InvalidOperationException>(() => post.Update("New Title", null, null));
    }

    [Fact]
    public void Remove_marks_post_as_removed()
    {
        var post = Post.Create("Title", null, null);

        post.Remove();

        Assert.True(post.Removed);
    }
}
