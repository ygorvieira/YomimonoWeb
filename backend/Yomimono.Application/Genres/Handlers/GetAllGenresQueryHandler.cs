using MediatR;
using Yomimono.Application.Common;
using Yomimono.Application.Genres.Common;
using Yomimono.Application.Genres.DTOs;
using Yomimono.Application.Genres.Queries;

namespace Yomimono.Application.Genres.Handlers;

public class GetAllGenresQueryHandler(IGenreRepository repository)
    : IRequestHandler<GetAllGenresQuery, Result<IEnumerable<GenreDto>>>
{
    public async Task<Result<IEnumerable<GenreDto>>> Handle(GetAllGenresQuery request, CancellationToken cancellationToken)
    {
        var genres = await repository.GetAllAsync(request.SearchTerm, cancellationToken);
        var dtos = genres.Select(g => new GenreDto(g.Id, g.Name, g.CreatedAt, g.UpdatedAt));
        return Result<IEnumerable<GenreDto>>.Success(dtos);
    }
}
