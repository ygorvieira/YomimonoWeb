using Yomimono.Domain.Common;
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
    IGenreRepository genreRepository,
    IBookUniquenessChecker uniquenessChecker)
    : IRequestHandler<CreateBookCommand, Result<BookDto>>
{
    public async Task<Result<BookDto>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        if (request.Book.Isbn is not null)
        {
            var isUnique = await uniquenessChecker.IsIsbnUniqueAsync(request.Book.Isbn, null, cancellationToken);
            if (!isUnique)
                return Result<BookDto>.Failure("Já existe um livro cadastrado com este ISBN.");
        }

        var genre = await genreRepository.GetByIdAsync(request.Book.GenreId, cancellationToken);
        if (genre is null)
            return Result<BookDto>.Failure("Gênero não encontrado.");

        var authorIds = request.Book.AuthorIds.Distinct().ToArray();
        if (authorIds.Length == 0)
            return Result<BookDto>.Failure("É necessário selecionar pelo menos um autor.");

        foreach (var authorId in authorIds)
        {
            var author = await authorRepository.GetByIdAsync(authorId, cancellationToken);
            if (author is null)
                return Result<BookDto>.Failure($"Autor com ID {authorId} não encontrado.");
        }

        var (book, error) = Book.Create(
            request.Book.Title, authorIds, request.Book.Isbn,
            request.Book.PublicationYear, request.Book.Publisher,
            request.Book.GenreId, request.Book.PageCount,
            request.Book.Description, request.Book.CoverUrl,
            request.Book.ReadingStatus, request.Book.IsLiked
        );

        if (error is not null)
            return Result<BookDto>.Failure(error);

        await bookRepository.AddAsync(book!, cancellationToken);
        return Result<BookDto>.Created(MapToDto(book!, genre), "Livro cadastrado com sucesso.");
    }

    private static BookDto MapToDto(Book book, Genre genre)
    {
        return new BookDto(
            book.Id, book.Title,
            book.BookAuthors.Select(ba => ba.AuthorId).ToArray(),
            book.BookAuthors.Select(ba => ba.Author?.Name ?? "").ToArray(),
            book.Isbn, book.PublicationYear, book.Publisher,
            book.GenreId, genre.Name,
            book.Description, book.PageCount, book.CoverUrl,
            book.ReadingStatus, book.IsLiked,
            book.CreatedAt, book.UpdatedAt
        );
    }
}
