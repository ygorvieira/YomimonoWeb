using MediatR;
using Yomimono.Application.Authors.DTOs;
using Yomimono.Application.Common;

namespace Yomimono.Application.Authors.Commands;

public record CreateAuthorCommand(CreateAuthorDto Author) : IRequest<Result<AuthorDto>>;
