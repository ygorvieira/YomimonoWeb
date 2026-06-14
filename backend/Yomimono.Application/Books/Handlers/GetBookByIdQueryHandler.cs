using MediatR;
using Yomimono.Application.Books.Common;
using Yomimono.Application.Books.DTOs;
using Yomimono.Application.Books.Queries;
using Yomimono.Application.Common;

namespace Yomimono.Application.Books.Handlers;

public class GetBookByIdQueryHandler(IBookRepository repository) : IRequestHandler<GetBookByIdQuery, Result<BookDto>>
{
    public async Task<Result<BookDto>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (book is null)
            return Result<BookDto>.NotFound("Livro não encontrado.");

        var dto = new BookDto(
            book.Id, book.Title, book.Author, book.Isbn,
            book.PublicationYear, book.Publisher, book.Genre,
            book.Description, book.PageCount, book.CoverUrl,
            book.CreatedAt, book.UpdatedAt
        );

        return Result<BookDto>.Success(dto);
    }
}
