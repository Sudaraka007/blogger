using FluentValidation;

namespace Blogger.Domain.UseCases.Posts.CreatePost;

public sealed class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(command => command.AuthorId)
            .GreaterThan(0)
            .WithMessage("AuthorId is required.");

        RuleFor(command => command.Title)
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage("Title is required.")
            .MaximumLength(300);

        RuleFor(command => command.Description)
            .MaximumLength(500)
            .When(command => command.Description is not null);
    }
}
