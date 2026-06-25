using Yomimono.Domain.Entities;
using MediatR;
using Yomimono.Application.Books.Commands;
using Yomimono.Application.Authors.Common;
using Yomimono.Application.Books.Common;
using Yomimono.Application.Books.DTOs;
using Yomimono.Application.Common;
using Yomimono.Application.Genres.Common;

namespace Yomimono.Application.Books.Handlers;

public class UpdateBookCommandHandler(
    IBookRepository bookRepository,
    IAuthorRepository authorRepository,
    IGenreRepository genreRepository)
    : IRequestHandler<UpdateBookCommand, Result<BookDto>>
{
    public async Task<Result<BookDto>> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var book = await bookRepository.GetByIdAsync(request.Id, cancellationToken);
        if (book is null)
            return Result<BookDto>.NotFound("Livro não encontrado.");

        List<Genre>? genres = null;
        if (request.Book.GenreIds is not null)
        {
            var genreIds = request.Book.GenreIds.Distinct().ToArray();
            if (genreIds.Length == 0)
                return Result<BookDto>.Failure("É necessário selecionar pelo menos um gênero.");

            genres = [];
            foreach (var genreId in genreIds)
            {
                var genre = await genreRepository.GetByIdAsync(genreId, cancellationToken);
                if (genre is null)
                    return Result<BookDto>.Failure($"Gênero com ID {genreId} não encontrado.");
                genres.Add(genre);
            }
        }

        if (request.Book.AuthorIds is not null || request.Book.OrganizerIds is not null)
        {
            var authorIds = request.Book.AuthorIds?.Distinct().ToArray() ?? [];
            var organizerIds = request.Book.OrganizerIds?.Distinct().ToArray() ?? [];

            if (authorIds.Length == 0 && organizerIds.Length == 0)
                return Result<BookDto>.Failure("É necessário selecionar pelo menos um autor ou organizador.");

            foreach (var id in authorIds.Concat(organizerIds))
            {
                var author = await authorRepository.GetByIdAsync(id, cancellationToken);
                if (author is null)
                    return Result<BookDto>.Failure($"Autor com ID {id} não encontrado.");
            }
        }

        var error = book.UpdateDetails(
            request.Book.Title, request.Book.AuthorIds,
            request.Book.PublicationYear, request.Book.Publisher,
            request.Book.GenreIds, request.Book.PageCount,
            request.Book.Description, request.Book.CoverUrl,
            request.Book.ReadingStatus, request.Book.IsLiked,
            request.Book.OrganizerIds,
            request.Book.IsTradePaperback, request.Book.TradeEdition,
            request.Book.IsDigital
        );

        if (error is not null)
            return Result<BookDto>.Failure(error);

        bookRepository.Update(book);

        var dto = MapToDto(book);
        return Result<BookDto>.Success(dto, "Livro atualizado com sucesso.");
    }

    private static BookDto MapToDto(Book book)
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
            book.IsTradePaperback, book.TradeEdition, book.IsDigital,
            book.CreatedAt, book.UpdatedAt
        );
    }
}
