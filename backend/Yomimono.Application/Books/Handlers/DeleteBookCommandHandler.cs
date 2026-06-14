using MediatR;
using Yomimono.Application.Books.Commands;
using Yomimono.Application.Books.Common;
using Yomimono.Application.Common;

namespace Yomimono.Application.Books.Handlers;

public class DeleteBookCommandHandler(IBookRepository repository) : IRequestHandler<DeleteBookCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var book = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (book is null)
            return Result<bool>.NotFound("Livro não encontrado.");

        repository.Delete(book);
        return Result<bool>.Success(true, "Livro removido com sucesso.");
    }
}
