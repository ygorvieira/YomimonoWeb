using MediatR;
using Yomimono.Application.Common;
using Yomimono.Application.Genres.DTOs;

namespace Yomimono.Application.Genres.Commands;

public record CreateGenreCommand(CreateGenreDto Genre) : IRequest<Result<GenreDto>>;
