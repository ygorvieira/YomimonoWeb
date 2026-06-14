using MediatR;
using Yomimono.Application.Books.Commands;
using Yomimono.Application.Books.Common;
using Yomimono.Application.Books.DTOs;
using Yomimono.Application.Common;
using Yomimono.Domain.Common;
using Yomimono.Domain.Entities;

namespace Yomimono.Application.Books.Handlers;

public class CreateBookCommandHandler(IBookRepository repository, IBookUniquenessChecker uniquenessChecker)
    : IRequestHandler<CreateBookCommand, Result<BookDto>>
{
    public async Task<Result<BookDto>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var isUnique = await uniquenessChecker.IsIsbnUniqueAsync(request.Book.Isbn, null, cancellationToken);
        if (!isUnique)
            return Result<BookDto>.Failure("Já existe um livro cadastrado com este ISBN.");

        var (book, error) = Book.Create(
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

        await repository.AddAsync(book!, cancellationToken);

        var dto = MapToDto(book!);
        return Result<BookDto>.Created(dto, "Livro cadastrado com sucesso.");
    }

    private static BookDto MapToDto(Book book) => new(
        book.Id, book.Title, book.Author, book.Isbn,
        book.PublicationYear, book.Publisher, book.Genre,
        book.Description, book.PageCount, book.CoverUrl,
        book.CreatedAt, book.UpdatedAt
    );
}
