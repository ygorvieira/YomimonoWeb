using Yomimono.Domain.Common;
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
    IGenreRepository genreRepository,
    IBookUniquenessChecker uniquenessChecker)
    : IRequestHandler<UpdateBookCommand, Result<BookDto>>
{
    public async Task<Result<BookDto>> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var book = await bookRepository.GetByIdAsync(request.Id, cancellationToken);
        if (book is null)
            return Result<BookDto>.NotFound("Livro não encontrado.");

        if (request.Book.Isbn is not null)
        {
            var isUnique = await uniquenessChecker.IsIsbnUniqueAsync(request.Book.Isbn, request.Id, cancellationToken);
            if (!isUnique)
                return Result<BookDto>.Failure("Já existe um livro cadastrado com este ISBN.");
        }

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

        if (request.Book.AuthorIds is not null)
        {
            var authorIds = request.Book.AuthorIds.Distinct().ToArray();
            if (authorIds.Length == 0)
                return Result<BookDto>.Failure("É necessário selecionar pelo menos um autor.");

            foreach (var authorId in authorIds)
            {
                var author = await authorRepository.GetByIdAsync(authorId, cancellationToken);
                if (author is null)
                    return Result<BookDto>.Failure($"Autor com ID {authorId} não encontrado.");
            }
        }

        var error = book.UpdateDetails(
            request.Book.Title, request.Book.AuthorIds, request.Book.Isbn,
            request.Book.PublicationYear, request.Book.Publisher,
            request.Book.GenreIds, request.Book.PageCount,
            request.Book.Description, request.Book.CoverUrl,
            request.Book.ReadingStatus, request.Book.IsLiked
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
