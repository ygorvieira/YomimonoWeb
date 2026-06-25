using MediatR;
using Yomimono.Application.Common;
using Yomimono.Application.Genres.DTOs;

namespace Yomimono.Application.Genres.Queries;

public record GetAllGenresQuery(string? SearchTerm = null) : IRequest<Result<IEnumerable<GenreDto>>>;
