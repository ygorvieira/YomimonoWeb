using MediatR;
using Yomimono.Application.Books.Common;
using Yomimono.Application.Books.DTOs;
using Yomimono.Application.Books.Queries;
using Yomimono.Application.Common;

namespace Yomimono.Application.Books.Handlers;

public class GetAllBooksQueryHandler(IBookRepository repository)
    : IRequestHandler<GetAllBooksQuery, Result<PagedResult<BookDto>>>
{
    public async Task<Result<PagedResult<BookDto>>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        var pagedBooks = await repository.GetAllPagedAsync(request.GenreId, request.AuthorId, request.ReadingStatus, request.PageNumber, request.PageSize, cancellationToken);

        var dtos = pagedBooks.Items.Select(MapToDto).ToList();

        var result = new PagedResult<BookDto>(
            dtos,
            pagedBooks.TotalCount,
            pagedBooks.PageNumber,
            pagedBooks.PageSize,
            pagedBooks.TotalPages,
            pagedBooks.HasNextPage,
            pagedBooks.HasPrevPage
        );

        return Result<PagedResult<BookDto>>.Success(result);
    }

    private static BookDto MapToDto(Domain.Entities.Book book)
    {
        return new BookDto(
            book.Id, book.Title,
            book.BookAuthors.Where(ba => ba.Role == "Author").Select(ba => ba.AuthorId).ToArray(),
            book.BookAuthors.Where(ba => ba.Role == "Author").Select(ba => ba.Author?.Name ?? "").ToArray(),
            book.BookAuthors.Where(ba => ba.Role == "Organizer").Select(ba => ba.AuthorId).ToArray(),
            book.BookAuthors.Where(ba => ba.Role == "Organizer").Select(ba => ba.Author?.Name ?? "").ToArray(),
            book.Isbn, book.PublicationYear, book.Publisher,
            book.Genres.Select(bg => bg.GenreId).ToArray(),
            book.Genres.Select(bg => bg.Genre?.Name ?? "").ToArray(),
            book.Description, book.PageCount, book.CoverUrl,
            book.ReadingStatus, book.IsLiked, book.ReReadCount,
            book.CreatedAt, book.UpdatedAt
        );
    }
}
