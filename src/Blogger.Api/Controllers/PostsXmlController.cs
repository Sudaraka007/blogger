using Blogger.Api.Mappers.Posts;
using Blogger.Api.XmlContracts.Posts;
using Blogger.Domain.UseCases.Posts.CreatePost;
using Blogger.Domain.UseCases.Posts.GetPostById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Blogger.Api.Controllers;

[ApiController]
[Route("api/xml/posts")]
[Produces("application/xml")]
public sealed class PostsXmlController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Consumes("application/xml")]
    [ProducesResponseType(typeof(PostXmlResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePostXmlRequest request,
        CancellationToken cancellationToken)
    {
        var post = await mediator.Send(
            new CreatePostCommand(request.AuthorId, request.Title, request.Description, request.Content),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = post.Id },
            PostXmlMapper.ToXml(post));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PostXmlResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        int id,
        [FromQuery] bool includeAuthor,
        CancellationToken cancellationToken)
    {
        var post = await mediator.Send(new GetPostByIdQuery(id, includeAuthor), cancellationToken);

        return post is null
            ? NotFound()
            : Ok(PostXmlMapper.ToXml(post));
    }
}
