using Blogger.Domain.Abstractions;
using Blogger.Domain.UseCases.Authors.CreateAuthor;
using Blogger.Domain.UseCases.Posts.CreatePost;
using Blogger.Domain.Models.Posts;
using Blogger.Domain.Tests.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Blogger.Domain.Tests.Abstractions;

public sealed class MonitoringBehaviorTests
{
    [Fact]
    public async Task Handle_logs_success_when_monitored_command_succeeds()
    {
        var logger = new TestLogger<MonitoringBehavior<CreateAuthorCommand, string>>();
        var behavior = new MonitoringBehavior<CreateAuthorCommand, string>(logger);
        var command = new CreateAuthorCommand("John", "Doe");

        var result = await behavior.Handle(
            command,
            _ => Task.FromResult("ok"),
            CancellationToken.None);

        Assert.Equal("ok", result);
        Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, logger.Entries[0].Level);
        Assert.Contains("CreateAuthorCommand succeeded", logger.Entries[0].Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Handle_logs_failure_when_monitored_command_throws()
    {
        var logger = new TestLogger<MonitoringBehavior<CreatePostCommand, Post>>();
        var behavior = new MonitoringBehavior<CreatePostCommand, Post>(logger);
        var command = new CreatePostCommand(1, "Title", "Description", "Sensitive content");
        var exception = new InvalidOperationException("failed");

        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            behavior.Handle(
                command,
                _ => throw exception,
                CancellationToken.None));

        Assert.Same(exception, thrown);
        Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, logger.Entries[0].Level);
        Assert.Same(exception, logger.Entries[0].Exception);
        Assert.Contains("CreatePostCommand failed", logger.Entries[0].Message, StringComparison.Ordinal);
        Assert.DoesNotContain("Sensitive content", logger.Entries[0].Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Handle_does_not_log_for_non_monitored_requests()
    {
        var logger = new TestLogger<MonitoringBehavior<NonMonitoredRequest, string>>();
        var behavior = new MonitoringBehavior<NonMonitoredRequest, string>(logger);

        var result = await behavior.Handle(
            new NonMonitoredRequest(),
            _ => Task.FromResult("ok"),
            CancellationToken.None);

        Assert.Equal("ok", result);
        Assert.Empty(logger.Entries);
    }

    private sealed record NonMonitoredRequest;
}
