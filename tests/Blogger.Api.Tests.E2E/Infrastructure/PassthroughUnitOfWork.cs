using Blogger.Domain.Abstractions;

namespace Blogger.Api.Tests.E2E.Infrastructure;

internal sealed class PassthroughUnitOfWork : IUnitOfWork
{
    public Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken = default) =>
        action(cancellationToken);
}
