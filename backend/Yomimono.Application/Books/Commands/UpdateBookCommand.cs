using MediatR;
using Yomimono.Application.Books.DTOs;
using Yomimono.Application.Common;

namespace Yomimono.Application.Books.Commands;

public record UpdateBookCommand(Guid Id, UpdateBookDto Book) : IRequest<Result<BookDto>>;
