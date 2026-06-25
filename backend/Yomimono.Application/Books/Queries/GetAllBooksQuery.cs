using MediatR;
using Yomimono.Application.Books.DTOs;
using Yomimono.Application.Common;

namespace Yomimono.Application.Books.Queries;

public record GetAllBooksQuery(
    Guid? GenreId = null,
    Guid? AuthorId = null,
    string? ReadingStatus = null,
    string? SearchTerm = null,
    int PageNumber = 1,
    int PageSize = 50
) : IRequest<Result<PagedResult<BookDto>>>;
