using System.Xml.Serialization;

namespace Blogger.Api.XmlContracts.ProblemDetails;

[XmlRoot("ValidationProblemDetails", Namespace = XmlProblemNamespaces.Contract)]
public sealed class ValidationProblemDetailsXmlResponse
{
    [XmlElement("Title")]
    public string? Title { get; set; }

    [XmlElement("Status")]
    public int? Status { get; set; }

    [XmlElement("Type")]
    public string? Type { get; set; }

    [XmlElement("Instance")]
    public string? Instance { get; set; }

    [XmlElement("TraceId")]
    public string? TraceId { get; set; }

    [XmlArray("Errors")]
    [XmlArrayItem("Error")]
    public List<ValidationErrorXmlResponse> Errors { get; set; } = [];
}

[XmlType(Namespace = XmlProblemNamespaces.Contract)]
public sealed class ValidationErrorXmlResponse
{
    [XmlElement("PropertyName")]
    public string PropertyName { get; set; } = string.Empty;

    [XmlElement("Message")]
    public string Message { get; set; } = string.Empty;
}
