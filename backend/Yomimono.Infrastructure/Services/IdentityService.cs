using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Yomimono.Application.Auth.DTOs;
using Yomimono.Application.Common;
using Yomimono.Domain.Entities;
using Yomimono.Infrastructure.Data;

namespace Yomimono.Infrastructure.Services;

public class IdentityService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    TokenService tokenService,
    AppDbContext dbContext)
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

        var accessToken = tokenService.GenerateToken(user);
        var response = await CreateAuthResponseWithRefreshTokenAsync(user, accessToken, cancellationToken);

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

        var accessToken = tokenService.GenerateToken(user);
        var response = await CreateAuthResponseWithRefreshTokenAsync(user, accessToken, cancellationToken);

        return Result<AuthResponse>.Success(response, "Login realizado com sucesso.");
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(RefreshRequestDto dto, CancellationToken cancellationToken = default)
    {
        var tokenHash = tokenService.HashToken(dto.RefreshToken);
        var storedToken = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == tokenHash, cancellationToken);

        if (storedToken is null || !storedToken.IsActive)
            return Result<AuthResponse>.Failure("Refresh token inválido ou expirado.", System.Net.HttpStatusCode.Unauthorized);

        var user = await userManager.FindByIdAsync(storedToken.UserId.ToString());
        if (user is null)
            return Result<AuthResponse>.Failure("Usuário não encontrado.", System.Net.HttpStatusCode.Unauthorized);

        storedToken.RevokedAt = DateTime.UtcNow;

        var accessToken = tokenService.GenerateToken(user);
        var response = await CreateAuthResponseWithRefreshTokenAsync(user, accessToken, cancellationToken);

        return Result<AuthResponse>.Success(response, "Token renovado com sucesso.");
    }

    public async Task<Result<bool>> RevokeTokenAsync(RefreshRequestDto dto, CancellationToken cancellationToken = default)
    {
        var tokenHash = tokenService.HashToken(dto.RefreshToken);
        var storedToken = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == tokenHash, cancellationToken);

        if (storedToken is not null)
        {
            storedToken.RevokedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result<bool>.Success(true, "Token revogado com sucesso.");
    }

    private async Task<AuthResponse> CreateAuthResponseWithRefreshTokenAsync(
        User user, string accessToken, CancellationToken cancellationToken)
    {
        var rawToken = tokenService.GenerateRefreshToken();
        var tokenHash = tokenService.HashToken(rawToken);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(tokenService.GetRefreshTokenExpireDays()),
            CreatedAt = DateTime.UtcNow
        };

        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponse(accessToken, rawToken, user.Email!, user.UserName!);
    }
}
