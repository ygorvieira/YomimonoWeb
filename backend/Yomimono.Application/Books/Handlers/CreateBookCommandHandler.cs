using MediatR;
using Yomimono.Application.Books.Commands;
using Yomimono.Application.Books.Common;
using Yomimono.Application.Books.DTOs;
using Yomimono.Application.Authors.Common;
using Yomimono.Application.Common;
using Yomimono.Application.Genres.Common;
using Yomimono.Domain.Entities;

namespace Yomimono.Application.Books.Handlers;

public class CreateBookCommandHandler(
    IBookRepository bookRepository,
    IAuthorRepository authorRepository,
    IGenreRepository genreRepository)
    : IRequestHandler<CreateBookCommand, Result<BookDto>>
{
    public async Task<Result<BookDto>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var genreIds = request.Book.GenreIds.Distinct().ToArray();
        if (genreIds.Length == 0)
            return Result<BookDto>.Failure("É necessário selecionar pelo menos um gênero.");

        var genres = new List<Genre>();
        foreach (var genreId in genreIds)
        {
            var genre = await genreRepository.GetByIdAsync(genreId, cancellationToken);
            if (genre is null)
                return Result<BookDto>.Failure($"Gênero com ID {genreId} não encontrado.");
            genres.Add(genre);
        }

        var authorIds = request.Book.AuthorIds.Distinct().ToArray();
        var organizerIds = request.Book.OrganizerIds?.Distinct().ToArray() ?? [];

        if (authorIds.Length == 0 && organizerIds.Length == 0)
            return Result<BookDto>.Failure("É necessário selecionar pelo menos um autor ou organizador.");

        foreach (var authorId in authorIds.Concat(organizerIds))
        {
            var author = await authorRepository.GetByIdAsync(authorId, cancellationToken);
            if (author is null)
                return Result<BookDto>.Failure($"Autor com ID {authorId} não encontrado.");
        }

        var (book, error) = Book.Create(
            request.Book.Title, authorIds,
            request.Book.PublicationYear, request.Book.Publisher,
            genreIds, request.Book.PageCount,
            request.Book.Description, request.Book.CoverUrl,
            request.Book.ReadingStatus, request.Book.IsLiked,
            organizerIds,
            request.Book.IsTradePaperback, request.Book.TradeEdition,
            request.Book.IsDigital
        );

        if (error is not null)
            return Result<BookDto>.Failure(error);

        await bookRepository.AddAsync(book!, cancellationToken);
        return Result<BookDto>.Created(MapToDto(book!, genres), "Livro cadastrado com sucesso.");
    }

    private static BookDto MapToDto(Book book, List<Genre> genres)
    {
        return new BookDto(
            book.Id, book.Title,
            book.BookAuthors.Where(ba => ba.Role == "Author").Select(ba => ba.AuthorId).ToArray(),
            book.BookAuthors.Where(ba => ba.Role == "Author").Select(ba => ba.Author?.Name ?? "").ToArray(),
            book.BookAuthors.Where(ba => ba.Role == "Organizer").Select(ba => ba.AuthorId).ToArray(),
            book.BookAuthors.Where(ba => ba.Role == "Organizer").Select(ba => ba.Author?.Name ?? "").ToArray(),
            book.PublicationYear, book.Publisher,
            genres.Select(g => g.Id).ToArray(),
            genres.Select(g => g.Name).ToArray(),
            book.Description, book.PageCount, book.CoverUrl,
            book.ReadingStatus, book.IsLiked, book.ReReadCount,
            book.IsTradePaperback, book.TradeEdition, book.IsDigital,
            book.CreatedAt, book.UpdatedAt
        );
    }
}
