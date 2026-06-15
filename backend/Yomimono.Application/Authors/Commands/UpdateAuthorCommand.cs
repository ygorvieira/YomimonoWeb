using MediatR;
using Yomimono.Application.Authors.DTOs;
using Yomimono.Application.Common;

namespace Yomimono.Application.Authors.Commands;

public record UpdateAuthorCommand(Guid Id, UpdateAuthorDto Author) : IRequest<Result<AuthorDto>>;
