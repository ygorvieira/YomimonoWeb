using Yomimono.Domain.Entities;
using MediatR;
using Yomimono.Application.Common;
using Yomimono.Application.Genres.Commands;
using Yomimono.Application.Genres.Common;
using Yomimono.Application.Genres.DTOs;

namespace Yomimono.Application.Genres.Handlers;

public class UpdateGenreCommandHandler(IGenreRepository repository)
    : IRequestHandler<UpdateGenreCommand, Result<GenreDto>>
{
    public async Task<Result<GenreDto>> Handle(UpdateGenreCommand request, CancellationToken cancellationToken)
    {
        var genre = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (genre is null)
            return Result<GenreDto>.NotFound("Gênero não encontrado.");

        var duplicate = await repository.GetByNameAsync(request.Genre.Name, cancellationToken);
        if (duplicate is not null && duplicate.Id != request.Id)
            return Result<GenreDto>.Failure("Já existe um gênero cadastrado com este nome.");

        var error = genre.UpdateName(request.Genre.Name);
        if (error is not null)
            return Result<GenreDto>.Failure(error);

        repository.Update(genre);
        return Result<GenreDto>.Success(MapToDto(genre), "Gênero atualizado com sucesso.");
    }

    private static GenreDto MapToDto(Genre genre) => new(genre.Id, genre.Name, genre.CreatedAt, genre.UpdatedAt);
}
