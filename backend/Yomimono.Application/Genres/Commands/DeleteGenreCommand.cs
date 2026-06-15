using MediatR;
using Yomimono.Application.Common;

namespace Yomimono.Application.Genres.Commands;

public record DeleteGenreCommand(Guid Id) : IRequest<Result<bool>>;
