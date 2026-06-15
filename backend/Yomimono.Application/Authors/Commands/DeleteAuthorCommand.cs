using MediatR;
using Yomimono.Application.Common;

namespace Yomimono.Application.Authors.Commands;

public record DeleteAuthorCommand(Guid Id) : IRequest<Result<bool>>;
