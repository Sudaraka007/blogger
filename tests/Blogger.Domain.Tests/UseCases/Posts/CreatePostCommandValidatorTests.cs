using Blogger.Domain.UseCases.Posts.CreatePost;
using FluentValidation.TestHelper;

namespace Blogger.Domain.Tests.UseCases.Posts;

public sealed class CreatePostCommandValidatorTests
{
    private readonly CreatePostCommandValidator _validator = new();

    [Fact]
    public void Should_pass_for_valid_command()
    {
        var command = new CreatePostCommand(1, "Title", "Description", "Content");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_fail_when_author_id_is_invalid(int authorId)
    {
        var command = new CreatePostCommand(authorId, "Title", null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AuthorId)
            .WithErrorMessage("AuthorId is required.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_fail_when_title_is_missing(string? title)
    {
        var command = new CreatePostCommand(1, title!, null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required.");
    }

    [Fact]
    public void Should_fail_when_title_exceeds_max_length()
    {
        var command = new CreatePostCommand(1, new string('a', 301), null, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_fail_when_description_exceeds_max_length()
    {
        var command = new CreatePostCommand(1, "Title", new string('a', 501), null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
