using MediatR;
using Yomimono.Application.Common;
using Yomimono.Application.Genres.Common;
using Yomimono.Application.Genres.DTOs;
using Yomimono.Application.Genres.Queries;

namespace Yomimono.Application.Genres.Handlers;

public class GetGenreByIdQueryHandler(IGenreRepository repository)
    : IRequestHandler<GetGenreByIdQuery, Result<GenreDto>>
{
    public async Task<Result<GenreDto>> Handle(GetGenreByIdQuery request, CancellationToken cancellationToken)
    {
        var genre = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (genre is null)
            return Result<GenreDto>.NotFound("Gênero não encontrado.");

        return Result<GenreDto>.Success(new GenreDto(genre.Id, genre.Name, genre.CreatedAt, genre.UpdatedAt));
    }
}
