using MediatR;
using Yomimono.Application.Books.Common;
using Yomimono.Application.Books.DTOs;
using Yomimono.Application.Books.Queries;
using Yomimono.Application.Common;

namespace Yomimono.Application.Books.Handlers;

public class GetAllBooksQueryHandler(IBookRepository repository) : IRequestHandler<GetAllBooksQuery, Result<IEnumerable<BookDto>>>
{
    public async Task<Result<IEnumerable<BookDto>>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        var books = await repository.GetAllAsync(cancellationToken);
        var dtos = books.Select(b => new BookDto(
            b.Id, b.Title, b.Author, b.Isbn,
            b.PublicationYear, b.Publisher, b.Genre,
            b.Description, b.PageCount, b.CoverUrl,
            b.CreatedAt, b.UpdatedAt
        ));

        return Result<IEnumerable<BookDto>>.Success(dtos);
    }
}
