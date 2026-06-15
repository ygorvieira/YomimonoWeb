using MediatR;
using Yomimono.Application.Authors.DTOs;
using Yomimono.Application.Common;

namespace Yomimono.Application.Authors.Queries;

public record GetAllAuthorsQuery : IRequest<Result<IEnumerable<AuthorDto>>>;
