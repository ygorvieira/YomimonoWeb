using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yomimono.Application.Books.Commands;
using Yomimono.Application.Books.DTOs;
using Yomimono.Application.Books.Queries;
using Yomimono.Application.Common;

namespace Yomimono.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BooksController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? genreId,
        [FromQuery] Guid? authorId,
        [FromQuery] string? readingStatus,
        [FromQuery] string? searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        if (pageSize > 100)
            return BadRequest(Result<object>.Failure("O tamanho da página não pode exceder 100 itens."));

        var result = await mediator.Send(new GetAllBooksQuery(genreId, authorId, readingStatus, searchTerm, page, pageSize));
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

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateBookStatusDto dto)
    {
        var result = await mediator.Send(new UpdateBookStatusCommand(id, dto));
        if (!result.Valid)
            return NotFound(result);
        return Ok(result);
    }
}
