namespace Yomimono.Application.Auth.DTOs;

public record RegisterDto(string Email, string Password, string UserName);
public record LoginDto(string Email, string Password);
public record AuthResponse(string Token, string Email, string UserName);
