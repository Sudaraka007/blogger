using Blogger.Api.Contracts.Posts;
using FluentValidation;

namespace Blogger.Api.Validators.Posts;

public sealed class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
    public CreatePostRequestValidator()
    {
        RuleFor(request => request.AuthorId)
            .GreaterThan(0)
            .WithMessage("AuthorId is required.");

        RuleFor(request => request.Title)
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage("Title is required.")
            .MaximumLength(300);

        RuleFor(request => request.Description)
            .MaximumLength(500)
            .When(request => request.Description is not null);
    }
}
