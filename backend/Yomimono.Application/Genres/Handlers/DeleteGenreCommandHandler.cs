using MediatR;
using Yomimono.Application.Common;
using Yomimono.Application.Genres.Commands;
using Yomimono.Application.Genres.Common;

namespace Yomimono.Application.Genres.Handlers;

public class DeleteGenreCommandHandler(IGenreRepository repository)
    : IRequestHandler<DeleteGenreCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteGenreCommand request, CancellationToken cancellationToken)
    {
        var genre = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (genre is null)
            return Result<bool>.NotFound("Gênero não encontrado.");

        repository.Delete(genre);
        return Result<bool>.Success(true, "Gênero excluído com sucesso.");
    }
}
