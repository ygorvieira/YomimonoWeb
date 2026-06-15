using MediatR;
using Yomimono.Application.Books.Common;
using Yomimono.Application.Books.DTOs;
using Yomimono.Application.Books.Queries;
using Yomimono.Application.Common;

namespace Yomimono.Application.Books.Handlers;

public class GetBookByIdQueryHandler(IBookRepository repository)
    : IRequestHandler<GetBookByIdQuery, Result<BookDto>>
{
    public async Task<Result<BookDto>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (book is null)
            return Result<BookDto>.NotFound("Livro não encontrado.");

        return Result<BookDto>.Success(MapToDto(book));
    }

    private static BookDto MapToDto(Domain.Entities.Book book)
    {
        return new BookDto(
            book.Id, book.Title,
            book.BookAuthors.Select(ba => ba.AuthorId).ToArray(),
            book.BookAuthors.Select(ba => ba.Author?.Name ?? "").ToArray(),
            book.Isbn, book.PublicationYear, book.Publisher,
            book.GenreId, book.Genre?.Name ?? "",
            book.Description, book.PageCount, book.CoverUrl,
            book.ReadingStatus, book.IsLiked,
            book.CreatedAt, book.UpdatedAt
        );
    }
}
