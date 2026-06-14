using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yomimono.Application.Books.Commands;
using Yomimono.Application.Books.DTOs;
using Yomimono.Application.Books.Queries;

namespace Yomimono.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BooksController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllBooksQuery());
        return result.Valid ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetBookByIdQuery(id));
        if (!result.Valid)
            return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookDto dto)
    {
        var result = await mediator.Send(new CreateBookCommand(dto));
        if (!result.Valid)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookDto dto)
    {
        var result = await mediator.Send(new UpdateBookCommand(id, dto));
        if (!result.Valid)
            return NotFound(result);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteBookCommand(id));
        if (!result.Valid)
            return NotFound(result);
        return Ok(result);
    }
}
