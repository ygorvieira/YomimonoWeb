using MediatR;
using Yomimono.Application.Auth.Commands;
using Yomimono.Application.Auth.DTOs;
using Yomimono.Application.Common;

namespace Yomimono.Application.Auth.Handlers;

public class RegisterCommandHandler(IIdentityService identityService)
    : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return await identityService.RegisterAsync(request.Dto, cancellationToken);
    }
}

public class LoginCommandHandler(IIdentityService identityService)
    : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await identityService.LoginAsync(request.Dto, cancellationToken);
    }
}

public class RefreshTokenCommandHandler(IIdentityService identityService)
    : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await identityService.RefreshTokenAsync(request.Dto, cancellationToken);
    }
}

public class RevokeTokenCommandHandler(IIdentityService identityService)
    : IRequestHandler<RevokeTokenCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        return await identityService.RevokeTokenAsync(request.Dto, cancellationToken);
    }
}
