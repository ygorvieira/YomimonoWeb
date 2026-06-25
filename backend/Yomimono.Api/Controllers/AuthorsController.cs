using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yomimono.Application.Authors.Commands;
using Yomimono.Application.Authors.DTOs;
using Yomimono.Application.Authors.Queries;

namespace Yomimono.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AuthorsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? searchTerm)
    {
        var result = await mediator.Send(new GetAllAuthorsQuery(searchTerm));
        return result.Valid ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetAuthorByIdQuery(id));
        if (!result.Valid)
            return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAuthorDto dto)
    {
        var result = await mediator.Send(new CreateAuthorCommand(dto));
        if (!result.Valid)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAuthorDto dto)
    {
        var result = await mediator.Send(new UpdateAuthorCommand(id, dto));
        if (!result.Valid)
            return NotFound(result);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteAuthorCommand(id));
        if (!result.Valid)
            return NotFound(result);
        return Ok(result);
    }
}
