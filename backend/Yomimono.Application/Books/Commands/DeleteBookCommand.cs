using MediatR;
using Yomimono.Application.Common;

namespace Yomimono.Application.Books.Commands;

public record DeleteBookCommand(Guid Id) : IRequest<Result<bool>>;
