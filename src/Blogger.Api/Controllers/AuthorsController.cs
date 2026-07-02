using Blogger.Api.Contracts.Authors;
using Blogger.Domain.UseCases.Authors.CreateAuthor;
using Blogger.Domain.UseCases.Authors.GetAuthorById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Blogger.Api.Controllers;

[ApiController]
[Route("api/authors")]
[Produces("application/json")]
public sealed class AuthorsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(AuthorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] CreateAuthorRequest request,
        CancellationToken cancellationToken)
    {
        var author = await mediator.Send(
            new CreateAuthorCommand(request.Name, request.Surname),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = author.Id },
            AuthorResponse.FromDomain(author));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AuthorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var author = await mediator.Send(new GetAuthorByIdQuery(id), cancellationToken);

        return author is null
            ? NotFound()
            : Ok(AuthorResponse.FromDomain(author));
    }
}
