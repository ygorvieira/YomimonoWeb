using MediatR;
using Yomimono.Application.Books.DTOs;
using Yomimono.Application.Common;

namespace Yomimono.Application.Books.Commands;

public record UpdateBookStatusCommand(Guid Id, UpdateBookStatusDto Status) : IRequest<Result<BookDto>>;
