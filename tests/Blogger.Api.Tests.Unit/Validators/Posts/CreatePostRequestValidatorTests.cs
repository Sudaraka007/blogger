using Blogger.Api.Contracts.Posts;
using Blogger.Api.Validators.Posts;
using FluentValidation.TestHelper;

namespace Blogger.Api.Tests.Unit.Validators.Posts;

public sealed class CreatePostRequestValidatorTests
{
    private readonly CreatePostRequestValidator _validator = new();

    [Fact]
    public void Should_pass_for_valid_request()
    {
        var request = new CreatePostRequest(1, "Title", "Description", "Content");

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_fail_when_author_id_is_invalid(int authorId)
    {
        var request = new CreatePostRequest(authorId, "Title", null, null);

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.AuthorId)
            .WithErrorMessage("AuthorId is required.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_fail_when_title_is_missing(string? title)
    {
        var request = new CreatePostRequest(1, title!, null, null);

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required.");
    }

    [Fact]
    public void Should_fail_when_description_exceeds_max_length()
    {
        var request = new CreatePostRequest(1, "Title", new string('a', 501), null);

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
