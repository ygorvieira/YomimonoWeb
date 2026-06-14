using MediatR;
using Yomimono.Application.Books.DTOs;
using Yomimono.Application.Common;

namespace Yomimono.Application.Books.Queries;

public record GetBookByIdQuery(Guid Id) : IRequest<Result<BookDto>>;
