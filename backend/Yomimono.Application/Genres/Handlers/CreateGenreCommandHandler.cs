using MediatR;
using Yomimono.Application.Common;
using Yomimono.Application.Genres.Commands;
using Yomimono.Application.Genres.Common;
using Yomimono.Application.Genres.DTOs;
using Yomimono.Domain.Entities;

namespace Yomimono.Application.Genres.Handlers;

public class CreateGenreCommandHandler(IGenreRepository repository)
    : IRequestHandler<CreateGenreCommand, Result<GenreDto>>
{
    public async Task<Result<GenreDto>> Handle(CreateGenreCommand request, CancellationToken cancellationToken)
    {
        var duplicate = await repository.GetByNameAsync(request.Genre.Name, cancellationToken);
        if (duplicate is not null)
            return Result<GenreDto>.Failure("Já existe um gênero cadastrado com este nome.");

        if (string.IsNullOrWhiteSpace(request.Genre.Name))
            return Result<GenreDto>.Failure("O campo nome é obrigatório.");
        if (request.Genre.Name.Length > 100)
            return Result<GenreDto>.Failure("O nome deve ter no máximo 100 caracteres.");

        var genre = Genre.Create(request.Genre.Name);
        await repository.AddAsync(genre, cancellationToken);
        return Result<GenreDto>.Created(MapToDto(genre), "Gênero cadastrado com sucesso.");
    }

    private static GenreDto MapToDto(Genre genre) => new(genre.Id, genre.Name, genre.CreatedAt, genre.UpdatedAt);
}
