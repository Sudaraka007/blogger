using Blogger.Api.Contracts.Authors;
using Blogger.Api.Validators.Authors;
using FluentValidation.TestHelper;

namespace Blogger.Api.Tests.Unit.Validators.Authors;

public sealed class CreateAuthorRequestValidatorTests
{
    private readonly CreateAuthorRequestValidator _validator = new();

    [Fact]
    public void Should_pass_for_valid_request()
    {
        var request = new CreateAuthorRequest("John", "Doe");

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_fail_when_name_is_missing(string? name)
    {
        var request = new CreateAuthorRequest(name!, "Doe");

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name is required.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_fail_when_surname_is_missing(string? surname)
    {
        var request = new CreateAuthorRequest("John", surname!);

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Surname)
            .WithErrorMessage("Surname is required.");
    }

    [Fact]
    public void Should_fail_when_name_exceeds_max_length()
    {
        var request = new CreateAuthorRequest(new string('a', 101), "Doe");

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
