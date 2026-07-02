using Blogger.Domain.Models.Authors;
using MediatR;

namespace Blogger.Domain.UseCases.Authors.CreateAuthor;

public sealed class CreateAuthorCommandHandler(IAuthorRepository authorRepository)
    : IRequestHandler<CreateAuthorCommand, Author>
{
    public async Task<Author> Handle(
        CreateAuthorCommand command,
        CancellationToken cancellationToken)
    {
        var author = Author.Create(command.Name, command.Surname);
        return await authorRepository.SaveAsync(author, cancellationToken);
    }
}
