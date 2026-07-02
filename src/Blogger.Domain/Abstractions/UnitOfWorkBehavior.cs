using MediatR;

namespace Blogger.Domain.Abstractions;

public sealed class UnitOfWorkBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IUnitOfWorkCommand)
        {
            return next(cancellationToken);
        }

        return unitOfWork.ExecuteInTransactionAsync(
            token => next(token),
            cancellationToken);
    }
}
