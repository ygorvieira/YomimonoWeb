using MediatR;
using Yomimono.Application.Auth.DTOs;
using Yomimono.Application.Common;

namespace Yomimono.Application.Auth.Commands;

public record LoginCommand(LoginDto Dto) : IRequest<Result<AuthResponse>>;
