using Blogger.Api.XmlContracts.Posts;
using Blogger.Domain.Models.Posts;
using Blogger.Domain.UseCases.Posts.GetPostById;

namespace Blogger.Api.Mappers.Posts;

public static class PostXmlMapper
{
    public static PostXmlResponse ToXml(Post post) =>
        new()
        {
            Id = post.Id,
            Title = post.Title,
            Description = post.Description,
            Content = post.Content
        };

    public static PostXmlResponse ToXml(PostDetails details) =>
        new()
        {
            Id = details.Id,
            Title = details.Title,
            Description = details.Description,
            Content = details.Content,
            Author = details.Author is null
                ? null
                : new PostAuthorXmlResponse
                {
                    Id = details.Author.Id,
                    Name = details.Author.Name,
                    Surname = details.Author.Surname
                }
        };
}
