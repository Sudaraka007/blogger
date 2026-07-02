using Blogger.Api.Contracts.Posts;
using Blogger.Domain.UseCases.Posts.CreatePost;
using Blogger.Domain.UseCases.Posts.GetPostById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Blogger.Api.Controllers;

[ApiController]
[Route("api/posts")]
[Produces("application/json")]
public sealed class PostsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(PostResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] CreatePostRequest request,
        CancellationToken cancellationToken)
    {
        var post = await mediator.Send(
            new CreatePostCommand(request.AuthorId, request.Title, request.Description, request.Content),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = post.Id },
            PostResponse.FromDomain(post));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PostResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        int id,
        [FromQuery] bool includeAuthor,
        CancellationToken cancellationToken)
    {
        var post = await mediator.Send(new GetPostByIdQuery(id, includeAuthor), cancellationToken);

        return post is null
            ? NotFound()
            : Ok(PostResponse.FromDetails(post));
    }
}
