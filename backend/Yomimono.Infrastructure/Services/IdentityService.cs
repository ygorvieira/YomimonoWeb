using Microsoft.AspNetCore.Identity;
using Yomimono.Application.Auth.DTOs;
using Yomimono.Application.Common;
using Yomimono.Domain.Entities;

namespace Yomimono.Infrastructure.Services;

public class IdentityService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    TokenService tokenService)
    : IIdentityService
{
    public async Task<Result<AuthResponse>> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        var existingUser = await userManager.FindByEmailAsync(dto.Email);
        if (existingUser is not null)
            return Result<AuthResponse>.Failure("Já existe um usuário com este email.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = dto.UserName,
            Email = dto.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToArray();
            return Result<AuthResponse>.Failure(string.Join(" ", errors));
        }

        var token = tokenService.GenerateToken(user);
        var response = new AuthResponse(token, user.Email!, user.UserName);

        return Result<AuthResponse>.Created(response, "Usuário cadastrado com sucesso.");
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return Result<AuthResponse>.Failure("Email ou senha inválidos.");

        var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
            return Result<AuthResponse>.Failure("Email ou senha inválidos.");

        var token = tokenService.GenerateToken(user);
        var response = new AuthResponse(token, user.Email!, user.UserName);

        return Result<AuthResponse>.Success(response, "Login realizado com sucesso.");
    }
}
