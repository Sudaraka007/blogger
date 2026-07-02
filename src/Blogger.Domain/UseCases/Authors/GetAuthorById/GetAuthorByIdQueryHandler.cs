using Blogger.Domain.Models.Authors;
using MediatR;

namespace Blogger.Domain.UseCases.Authors.GetAuthorById;

public sealed class GetAuthorByIdQueryHandler(IAuthorRepository authorRepository)
    : IRequestHandler<GetAuthorByIdQuery, Author?>
{
    public Task<Author?> Handle(
        GetAuthorByIdQuery query,
        CancellationToken cancellationToken) =>
        authorRepository.GetByIdAsync(query.Id, cancellationToken);
}
