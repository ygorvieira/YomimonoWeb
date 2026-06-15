using MediatR;
using Yomimono.Application.Books.DTOs;
using Yomimono.Application.Common;

namespace Yomimono.Application.Books.Queries;

public record GetAllBooksQuery(
    Guid? GenreId = null,
    Guid? AuthorId = null,
    string? ReadingStatus = null
) : IRequest<Result<IEnumerable<BookDto>>>;
