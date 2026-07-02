using Blogger.Domain.Abstractions;
using Blogger.Domain.UseCases.Authors.CreateAuthor;
using Blogger.Domain.UseCases.Authors.GetAuthorById;
using Blogger.Domain.UseCases.Posts.CreatePost;
using Blogger.Domain.UseCases.Posts.GetPostById;
using Blogger.Domain.Tests.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Blogger.Domain.Tests.UseCases;

public sealed class MonitoredCommandLoggingTests
{
    [Fact]
    public void CreateAuthorCommand_logs_success_with_command_details()
    {
        var logger = new TestLogger<CreateAuthorCommand>();
        IMonitoredCommand command = new CreateAuthorCommand("John", "Doe");

        command.LogSuccess(logger);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, entry.Level);
        Assert.Contains("CreateAuthorCommand succeeded", entry.Message, StringComparison.Ordinal);
        Assert.Contains("John", entry.Message, StringComparison.Ordinal);
        Assert.Contains("Doe", entry.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void CreateAuthorCommand_logs_failure_with_command_details()
    {
        var logger = new TestLogger<CreateAuthorCommand>();
        IMonitoredCommand command = new CreateAuthorCommand("John", "Doe");
        var exception = new InvalidOperationException("failed");

        command.LogFailure(logger, exception);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Same(exception, entry.Exception);
        Assert.Contains("CreateAuthorCommand failed", entry.Message, StringComparison.Ordinal);
        Assert.Contains("John", entry.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GetAuthorByIdQuery_logs_success_with_query_id()
    {
        var logger = new TestLogger<GetAuthorByIdQuery>();
        IMonitoredCommand query = new GetAuthorByIdQuery(42);

        query.LogSuccess(logger);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, entry.Level);
        Assert.Contains("GetAuthorByIdQuery succeeded", entry.Message, StringComparison.Ordinal);
        Assert.Contains("42", entry.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GetAuthorByIdQuery_logs_failure_with_query_id()
    {
        var logger = new TestLogger<GetAuthorByIdQuery>();
        IMonitoredCommand query = new GetAuthorByIdQuery(42);
        var exception = new InvalidOperationException("failed");

        query.LogFailure(logger, exception);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Contains("GetAuthorByIdQuery failed", entry.Message, StringComparison.Ordinal);
        Assert.Contains("42", entry.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GetPostByIdQuery_logs_success_with_query_details()
    {
        var logger = new TestLogger<GetPostByIdQuery>();
        IMonitoredCommand query = new GetPostByIdQuery(7, IncludeAuthor: true);

        query.LogSuccess(logger);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, entry.Level);
        Assert.Contains("GetPostByIdQuery succeeded", entry.Message, StringComparison.Ordinal);
        Assert.Contains("7", entry.Message, StringComparison.Ordinal);
        Assert.Contains("True", entry.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void GetPostByIdQuery_logs_failure_with_query_details()
    {
        var logger = new TestLogger<GetPostByIdQuery>();
        IMonitoredCommand query = new GetPostByIdQuery(7, IncludeAuthor: true);
        var exception = new InvalidOperationException("failed");

        query.LogFailure(logger, exception);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Contains("GetPostByIdQuery failed", entry.Message, StringComparison.Ordinal);
        Assert.Contains("7", entry.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void CreatePostCommand_logs_success_without_post_content()
    {
        var logger = new TestLogger<CreatePostCommand>();
        var command = new CreatePostCommand(1, "Title", "Description", "Sensitive content");

        command.LogSuccess(logger);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, entry.Level);
        Assert.Equal(
            "CreatePostCommand succeeded for AuthorId 1 with title Title",
            entry.Message);
        Assert.DoesNotContain("Sensitive content", entry.Message, StringComparison.Ordinal);
        Assert.DoesNotContain("Description", entry.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void CreatePostCommand_logs_failure_without_post_content()
    {
        var logger = new TestLogger<CreatePostCommand>();
        var command = new CreatePostCommand(1, "Title", "Description", "Sensitive content");
        var exception = new InvalidOperationException("failed");

        command.LogFailure(logger, exception);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.Same(exception, entry.Exception);
        Assert.Equal(
            "CreatePostCommand failed for AuthorId 1 with title Title",
            entry.Message);
        Assert.DoesNotContain("Sensitive content", entry.Message, StringComparison.Ordinal);
    }
}
