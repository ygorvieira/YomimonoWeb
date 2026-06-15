using MediatR;
using Yomimono.Application.Auth.DTOs;
using Yomimono.Application.Common;

namespace Yomimono.Application.Auth.Commands;

public record RevokeTokenCommand(RefreshRequestDto Dto) : IRequest<Result<bool>>;
