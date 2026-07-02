using System.Xml.Serialization;

namespace Blogger.Api.XmlContracts.Posts;

public sealed class PostAuthorXmlResponse
{
    [XmlElement("Id")]
    public int Id { get; set; }

    [XmlElement("Name")]
    public string Name { get; set; } = string.Empty;

    [XmlElement("Surname")]
    public string Surname { get; set; } = string.Empty;
}
