using System.Xml.Serialization;

namespace Blogger.Api.XmlContracts.Posts;

[XmlRoot("CreatePostRequest", Namespace = XmlPostNamespaces.Contract)]
public sealed class CreatePostXmlRequest
{
    [XmlElement("AuthorId")]
    public int AuthorId { get; set; }

    [XmlElement("Title")]
    public string Title { get; set; } = string.Empty;

    [XmlElement("Description")]
    public string? Description { get; set; }

    [XmlElement("Content")]
    public string? Content { get; set; }
}
