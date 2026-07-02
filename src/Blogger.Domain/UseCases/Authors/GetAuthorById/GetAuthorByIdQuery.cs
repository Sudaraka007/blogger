using Blogger.Domain.Abstractions;
using Blogger.Domain.Models.Authors;
using MediatR;

namespace Blogger.Domain.UseCases.Authors.GetAuthorById;

public sealed record GetAuthorByIdQuery(int Id) : IRequest<Author?>, IMonitoredCommand;
