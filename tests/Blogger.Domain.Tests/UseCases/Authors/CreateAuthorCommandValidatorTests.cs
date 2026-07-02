using Blogger.Domain.UseCases.Authors.CreateAuthor;
using FluentValidation.TestHelper;

namespace Blogger.Domain.Tests.UseCases.Authors;

public sealed class CreateAuthorCommandValidatorTests
{
    private readonly CreateAuthorCommandValidator _validator = new();

    [Fact]
    public void Should_pass_for_valid_command()
    {
        var command = new CreateAuthorCommand("John", "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_fail_when_name_is_missing(string? name)
    {
        var command = new CreateAuthorCommand(name!, "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name is required.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_fail_when_surname_is_missing(string? surname)
    {
        var command = new CreateAuthorCommand("John", surname!);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Surname)
            .WithErrorMessage("Surname is required.");
    }

    [Fact]
    public void Should_fail_when_name_exceeds_max_length()
    {
        var command = new CreateAuthorCommand(new string('a', 101), "Doe");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
