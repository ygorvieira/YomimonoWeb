using MediatR;
using Yomimono.Application.Authors.DTOs;
using Yomimono.Application.Common;

namespace Yomimono.Application.Authors.Queries;

public record GetAllAuthorsQuery(string? SearchTerm = null) : IRequest<Result<IEnumerable<AuthorDto>>>;
