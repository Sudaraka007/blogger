using Blogger.Api.Contracts.Authors;
using FluentValidation;

namespace Blogger.Api.Validators.Authors;

public sealed class CreateAuthorRequestValidator : AbstractValidator<CreateAuthorRequest>
{
    public CreateAuthorRequestValidator()
    {
        RuleFor(request => request.Name)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Name is required.")
            .MaximumLength(100);

        RuleFor(request => request.Surname)
            .Must(surname => !string.IsNullOrWhiteSpace(surname))
            .WithMessage("Surname is required.")
            .MaximumLength(100);
    }
}
