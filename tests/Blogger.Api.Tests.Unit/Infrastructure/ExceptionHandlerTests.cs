using System.Text;
using System.Xml.Linq;
using Blogger.Api.Infrastructure;
using Blogger.Api.XmlContracts.ProblemDetails;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blogger.Api.Tests.Unit.Infrastructure;

public sealed class ExceptionHandlerTests
{
    [Fact]
    public async Task ValidationExceptionHandler_writes_json_problem_details_with_trace_id()
    {
        var handler = new ValidationExceptionHandler();
        var httpContext = CreateHttpContext("/api/posts", acceptsXml: false);
        var exception = new ValidationException(
        [
            new ValidationFailure("Title", "Title is required.")
        ]);

        var handled = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", httpContext.Response.ContentType);

        var body = await ReadResponseBodyAsync(httpContext);
        Assert.Contains("\"traceId\"", body, StringComparison.Ordinal);
        Assert.Contains("Title is required.", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ValidationExceptionHandler_writes_xml_problem_details_for_xml_routes()
    {
        var handler = new ValidationExceptionHandler();
        var httpContext = CreateHttpContext("/api/xml/posts", acceptsXml: false);
        var exception = new ValidationException(
        [
            new ValidationFailure("AuthorId", "Author 9999 was not found.")
        ]);

        var handled = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);
        Assert.Equal("application/xml; charset=utf-8", httpContext.Response.ContentType);

        var document = XDocument.Parse(await ReadResponseBodyAsync(httpContext));
        var root = document.Root;

        Assert.NotNull(root);
        Assert.Equal("ValidationProblemDetails", root.Name.LocalName);
        Assert.Equal(XmlProblemNamespaces.Contract, root.Name.NamespaceName);
        Assert.NotNull(root.Element(XName.Get("TraceId", XmlProblemNamespaces.Contract))?.Value);
    }

    [Fact]
    public async Task NotFoundExceptionHandler_writes_xml_problem_details_when_client_accepts_xml()
    {
        var handler = new NotFoundExceptionHandler();
        var httpContext = CreateHttpContext("/api/posts/1", acceptsXml: true);
        var exception = new InvalidOperationException("Post 1 was not found.");

        var handled = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status404NotFound, httpContext.Response.StatusCode);
        Assert.Equal("application/xml; charset=utf-8", httpContext.Response.ContentType);

        var document = XDocument.Parse(await ReadResponseBodyAsync(httpContext));
        var root = document.Root;

        Assert.NotNull(root);
        Assert.Equal("ProblemDetails", root.Name.LocalName);
        Assert.Equal("Post 1 was not found.", root.Element(XName.Get("Detail", XmlProblemNamespaces.Contract))?.Value);
    }

    [Fact]
    public async Task DbUpdateExceptionHandler_maps_foreign_key_violation_to_bad_request()
    {
        var handler = new DbUpdateExceptionHandler();
        var httpContext = CreateHttpContext("/api/posts", acceptsXml: false);
        var exception = new Microsoft.EntityFrameworkCore.DbUpdateException(
            "Save failed.",
            new InvalidOperationException("Cannot add or update a child row: a foreign key constraint fails"));

        var handled = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);

        var body = await ReadResponseBodyAsync(httpContext);
        Assert.Contains("related resource is missing or invalid", body, StringComparison.Ordinal);
    }

    private static DefaultHttpContext CreateHttpContext(string path, bool acceptsXml)
    {
        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                Path = path
            },
            Response =
            {
                Body = new MemoryStream()
            }
        };

        if (acceptsXml)
        {
            httpContext.Request.Headers.Accept = "application/xml";
        }

        return httpContext;
    }

    private static async Task<string> ReadResponseBodyAsync(HttpContext httpContext)
    {
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(httpContext.Response.Body, Encoding.UTF8, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }
}
