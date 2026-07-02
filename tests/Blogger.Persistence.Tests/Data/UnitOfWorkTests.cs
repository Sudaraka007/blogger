using Blogger.Persistence.Data;
using Blogger.Persistence.Tests.Infrastructure;

namespace Blogger.Persistence.Tests.Data;

public sealed class UnitOfWorkTests : IDisposable
{
    private readonly PersistenceTestContext _context = new();
    private readonly UnitOfWork _unitOfWork;

    public UnitOfWorkTests()
    {
        _unitOfWork = new UnitOfWork(_context.DbContext);
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_returns_result()
    {
        var result = await _unitOfWork.ExecuteInTransactionAsync(
            _ => Task.FromResult("completed"),
            CancellationToken.None);

        Assert.Equal("completed", result);
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_rethrows_exception()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _unitOfWork.ExecuteInTransactionAsync<int>(
                _ => throw new InvalidOperationException("failed"),
                CancellationToken.None));
    }

    public void Dispose() => _context.Dispose();
}
