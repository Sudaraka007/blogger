using Blogger.Api.Validators.Posts;
using Blogger.Api.XmlContracts.Posts;
using FluentValidation.TestHelper;

namespace Blogger.Api.Tests.Unit.Validators.Posts;

public sealed class CreatePostXmlRequestValidatorTests
{
    private readonly CreatePostXmlRequestValidator _validator = new();

    [Fact]
    public void Should_pass_for_valid_request()
    {
        var request = new CreatePostXmlRequest
        {
            AuthorId = 1,
            Title = "Title",
            Description = "Description",
            Content = "Content"
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_fail_when_author_id_is_invalid(int authorId)
    {
        var request = new CreatePostXmlRequest
        {
            AuthorId = authorId,
            Title = "Title"
        };

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
        var request = new CreatePostXmlRequest
        {
            AuthorId = 1,
            Title = title!
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required.");
    }
}
