using Blogger.Domain.Abstractions;
using Blogger.Domain.Models.Authors;
using MediatR;

namespace Blogger.Domain.UseCases.Authors.CreateAuthor;

public sealed record CreateAuthorCommand(string Name, string Surname) : IRequest<Author>, IUnitOfWorkCommand, IMonitoredCommand;
