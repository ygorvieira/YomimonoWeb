using Yomimono.Application.Auth.DTOs;
using Yomimono.Application.Common;

namespace Yomimono.Application.Common;

public interface IIdentityService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
}
