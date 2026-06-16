using MediatR;
using Yomimono.Application.Books.Common;
using Yomimono.Application.Books.DTOs;
using Yomimono.Application.Books.Queries;
using Yomimono.Application.Common;

namespace Yomimono.Application.Books.Handlers;

public class GetAllBooksQueryHandler(IBookRepository repository)
    : IRequestHandler<GetAllBooksQuery, Result<IEnumerable<BookDto>>>
{
    public async Task<Result<IEnumerable<BookDto>>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        var books = await repository.GetAllAsync(request.GenreId, request.AuthorId, request.ReadingStatus, cancellationToken);
        var dtos = books.Select(MapToDto);
        return Result<IEnumerable<BookDto>>.Success(dtos);
    }

    private static BookDto MapToDto(Domain.Entities.Book book)
    {
        return new BookDto(
            book.Id, book.Title,
            book.BookAuthors.Select(ba => ba.AuthorId).ToArray(),
            book.BookAuthors.Select(ba => ba.Author?.Name ?? "").ToArray(),
            book.Isbn, book.PublicationYear, book.Publisher,
            book.Genres.Select(bg => bg.GenreId).ToArray(),
            book.Genres.Select(bg => bg.Genre?.Name ?? "").ToArray(),
            book.Description, book.PageCount, book.CoverUrl,
            book.ReadingStatus, book.IsLiked, book.ReReadCount,
            book.CreatedAt, book.UpdatedAt
        );
    }
}
