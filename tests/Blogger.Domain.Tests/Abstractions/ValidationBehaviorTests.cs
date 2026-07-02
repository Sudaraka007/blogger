using Blogger.Domain.Abstractions;
using FluentValidation;
using MediatR;

namespace Blogger.Domain.Tests.Abstractions;

public sealed class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_calls_next_when_no_validators_are_registered()
    {
        var behavior = new ValidationBehavior<TestRequest, string>([]);
        var nextCalled = false;

        var result = await behavior.Handle(
            new TestRequest(),
            _ =>
            {
                nextCalled = true;
                return Task.FromResult("ok");
            },
            CancellationToken.None);

        Assert.True(nextCalled);
        Assert.Equal("ok", result);
    }

    [Fact]
    public async Task Handle_calls_next_when_validation_passes()
    {
        var behavior = new ValidationBehavior<TestRequest, string>([new PassingTestValidator()]);

        var result = await behavior.Handle(
            new TestRequest(),
            _ => Task.FromResult("ok"),
            CancellationToken.None);

        Assert.Equal("ok", result);
    }

    [Fact]
    public async Task Handle_throws_when_validation_fails()
    {
        var behavior = new ValidationBehavior<TestRequest, string>([new FailingTestValidator()]);

        await Assert.ThrowsAsync<ValidationException>(() =>
            behavior.Handle(
                new TestRequest(),
                _ => Task.FromResult("ok"),
                CancellationToken.None));
    }

    public sealed record TestRequest;

    private sealed class PassingTestValidator : AbstractValidator<TestRequest>;

    private sealed class FailingTestValidator : AbstractValidator<TestRequest>
    {
        public FailingTestValidator()
        {
            RuleFor(request => request)
                .Must(_ => false)
                .WithMessage("Error message");
        }
    }
}
