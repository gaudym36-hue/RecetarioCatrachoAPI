using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Recetario.Application;

namespace Recetario.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecetasController : ControllerBase
{
    private readonly IRecetaService _service;
    public RecetasController(IRecetaService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<RecetaResponseDto>>> GetAll(
        [FromQuery] string? search, [FromQuery] int? categoriaId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(await _service.GetAllAsync(search, categoriaId, page, pageSize));
    }

    [HttpGet("{id:guid}", Name = "GetRecetaById")]
    public async Task<ActionResult<RecetaResponseDto>> GetById(Guid id)
    {
        return Ok(await _service.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult<RecetaResponseDto>> Create([FromBody] CreateRecetaDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtRoute("GetRecetaById", new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRecetaDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
