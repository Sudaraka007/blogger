using System.Xml.Serialization;

namespace Blogger.Api.XmlContracts.Posts;

[XmlRoot("PostResponse", Namespace = XmlPostNamespaces.Contract)]
public sealed class PostXmlResponse
{
    [XmlElement("Id")]
    public int Id { get; set; }

    [XmlElement("Title")]
    public string Title { get; set; } = string.Empty;

    [XmlElement("Description")]
    public string? Description { get; set; }

    [XmlElement("Content")]
    public string? Content { get; set; }

    [XmlElement("Author")]
    public PostAuthorXmlResponse? Author { get; set; }
}
