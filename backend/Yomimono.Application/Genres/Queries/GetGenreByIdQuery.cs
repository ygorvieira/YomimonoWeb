using MediatR;
using Yomimono.Application.Common;
using Yomimono.Application.Genres.DTOs;

namespace Yomimono.Application.Genres.Queries;

public record GetGenreByIdQuery(Guid Id) : IRequest<Result<GenreDto>>;
