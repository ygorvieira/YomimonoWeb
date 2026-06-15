using MediatR;
using Yomimono.Application.Authors.Commands;
using Yomimono.Application.Authors.Common;
using Yomimono.Application.Common;

namespace Yomimono.Application.Authors.Handlers;

public class DeleteAuthorCommandHandler(IAuthorRepository repository)
    : IRequestHandler<DeleteAuthorCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (author is null)
            return Result<bool>.NotFound("Autor não encontrado.");

        repository.Delete(author);
        return Result<bool>.Success(true, "Autor excluído com sucesso.");
    }
}
