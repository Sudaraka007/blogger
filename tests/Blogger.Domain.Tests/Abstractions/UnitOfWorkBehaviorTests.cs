using Blogger.Domain.Abstractions;
using Blogger.Domain.UseCases.Authors.CreateAuthor;
using MediatR;
using Moq;

namespace Blogger.Domain.Tests.Abstractions;

public sealed class UnitOfWorkBehaviorTests
{
    [Fact]
    public async Task Handle_executes_in_transaction_for_unit_of_work_commands()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork
            .Setup(work => work.ExecuteInTransactionAsync(
                It.IsAny<Func<CancellationToken, Task<string>>>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<string>>, CancellationToken>(
                (action, token) => action(token));

        var behavior = new UnitOfWorkBehavior<CreateAuthorCommand, string>(unitOfWork.Object);
        var command = new CreateAuthorCommand("John", "Doe");

        var result = await behavior.Handle(
            command,
            _ => Task.FromResult("saved"),
            CancellationToken.None);

        Assert.Equal("saved", result);
        unitOfWork.Verify(
            work => work.ExecuteInTransactionAsync(
                It.IsAny<Func<CancellationToken, Task<string>>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_skips_transaction_for_non_unit_of_work_requests()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var behavior = new UnitOfWorkBehavior<NonTransactionalRequest, string>(unitOfWork.Object);

        var result = await behavior.Handle(
            new NonTransactionalRequest(),
            _ => Task.FromResult("ok"),
            CancellationToken.None);

        Assert.Equal("ok", result);
        unitOfWork.Verify(
            work => work.ExecuteInTransactionAsync(
                It.IsAny<Func<CancellationToken, Task<string>>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    public sealed record NonTransactionalRequest;
}
