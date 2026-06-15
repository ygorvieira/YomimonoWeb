using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yomimono.Application.Genres.Commands;
using Yomimono.Application.Genres.DTOs;
using Yomimono.Application.Genres.Queries;

namespace Yomimono.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GenresController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllGenresQuery());
        return result.Valid ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetGenreByIdQuery(id));
        if (!result.Valid)
            return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGenreDto dto)
    {
        var result = await mediator.Send(new CreateGenreCommand(dto));
        if (!result.Valid)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGenreDto dto)
    {
        var result = await mediator.Send(new UpdateGenreCommand(id, dto));
        if (!result.Valid)
            return NotFound(result);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteGenreCommand(id));
        if (!result.Valid)
            return NotFound(result);
        return Ok(result);
    }
}
