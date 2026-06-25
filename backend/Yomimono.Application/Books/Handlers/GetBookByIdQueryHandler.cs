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
            book.BookAuthors.Where(ba => ba.Role == "Author").Select(ba => ba.AuthorId).ToArray(),
            book.BookAuthors.Where(ba => ba.Role == "Author").Select(ba => ba.Author?.Name ?? "").ToArray(),
            book.BookAuthors.Where(ba => ba.Role == "Organizer").Select(ba => ba.AuthorId).ToArray(),
            book.BookAuthors.Where(ba => ba.Role == "Organizer").Select(ba => ba.Author?.Name ?? "").ToArray(),
            book.PublicationYear, book.Publisher,
            book.Genres.Select(bg => bg.GenreId).ToArray(),
            book.Genres.Select(bg => bg.Genre?.Name ?? "").ToArray(),
            book.Description, book.PageCount, book.CoverUrl,
            book.ReadingStatus, book.IsLiked, book.ReReadCount,
            book.IsTradePaperback,
            book.BookEditions.Select(e => new BookEditionDto(e.Name, e.Number)).ToArray(),
            book.IsDigital,
            book.CreatedAt, book.UpdatedAt
        );
    }
}
