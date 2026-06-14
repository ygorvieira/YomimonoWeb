using MediatR;
using Yomimono.Application.Books.DTOs;
using Yomimono.Application.Common;

namespace Yomimono.Application.Books.Commands;

public record CreateBookCommand(CreateBookDto Book) : IRequest<Result<BookDto>>;
