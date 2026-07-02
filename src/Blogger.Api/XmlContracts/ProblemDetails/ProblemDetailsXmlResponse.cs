using System.Xml.Serialization;

namespace Blogger.Api.XmlContracts.ProblemDetails;

[XmlRoot("ProblemDetails", Namespace = XmlProblemNamespaces.Contract)]
public sealed class ProblemDetailsXmlResponse
{
    [XmlElement("Title")]
    public string? Title { get; set; }

    [XmlElement("Status")]
    public int? Status { get; set; }

    [XmlElement("Detail")]
    public string? Detail { get; set; }

    [XmlElement("Type")]
    public string? Type { get; set; }

    [XmlElement("Instance")]
    public string? Instance { get; set; }

    [XmlElement("TraceId")]
    public string? TraceId { get; set; }
}
