using FluentValidation;

namespace Blogger.Domain.UseCases.Authors.CreateAuthor;

public sealed class CreateAuthorCommandValidator : AbstractValidator<CreateAuthorCommand>
{
    public CreateAuthorCommandValidator()
    {
        RuleFor(command => command.Name)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Name is required.")
            .MaximumLength(100);

        RuleFor(command => command.Surname)
            .Must(surname => !string.IsNullOrWhiteSpace(surname))
            .WithMessage("Surname is required.")
            .MaximumLength(100);
    }
}
