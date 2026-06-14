using MediatR;
using Yomimono.Application.Books.Commands;
using Yomimono.Application.Books.Common;
using Yomimono.Application.Books.DTOs;
using Yomimono.Application.Common;
using Yomimono.Domain.Common;

namespace Yomimono.Application.Books.Handlers;

public class UpdateBookCommandHandler(IBookRepository repository, IBookUniquenessChecker uniquenessChecker)
    : IRequestHandler<UpdateBookCommand, Result<BookDto>>
{
    public async Task<Result<BookDto>> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var book = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (book is null)
            return Result<BookDto>.NotFound("Livro não encontrado.");

        if (request.Book.Isbn is not null)
        {
            var isUnique = await uniquenessChecker.IsIsbnUniqueAsync(request.Book.Isbn, request.Id, cancellationToken);
            if (!isUnique)
                return Result<BookDto>.Failure("Já existe outro livro cadastrado com este ISBN.");
        }

        var error = book.UpdateDetails(
            request.Book.Title,
            request.Book.Author,
            request.Book.Isbn,
            request.Book.PublicationYear,
            request.Book.Publisher,
            request.Book.Genre,
            request.Book.PageCount,
            request.Book.Description,
            request.Book.CoverUrl
        );

        if (error is not null)
            return Result<BookDto>.Failure(error);

        repository.Update(book);

        var dto = new BookDto(
            book.Id, book.Title, book.Author, book.Isbn,
            book.PublicationYear, book.Publisher, book.Genre,
            book.Description, book.PageCount, book.CoverUrl,
            book.CreatedAt, book.UpdatedAt
        );

        return Result<BookDto>.Success(dto, "Livro atualizado com sucesso.");
    }
}
