using MediatR;
using Yomimono.Application.Common;
using Yomimono.Application.Genres.DTOs;

namespace Yomimono.Application.Genres.Commands;

public record UpdateGenreCommand(Guid Id, UpdateGenreDto Genre) : IRequest<Result<GenreDto>>;
