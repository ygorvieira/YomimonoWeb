using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yomimono.Application.Auth.Commands;
using Yomimono.Application.Auth.DTOs;

namespace Yomimono.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await mediator.Send(new RegisterCommand(dto));
        if (!result.Valid)
            return BadRequest(result);
        return CreatedAtAction(nameof(Login), result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await mediator.Send(new LoginCommand(dto));
        if (!result.Valid)
            return Unauthorized(result);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
    {
        var result = await mediator.Send(new RefreshTokenCommand(dto));
        if (!result.Valid)
            return Unauthorized(result);
        return Ok(result);
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke([FromBody] RefreshRequestDto dto)
    {
        var result = await mediator.Send(new RevokeTokenCommand(dto));
        return Ok(result);
    }
}
