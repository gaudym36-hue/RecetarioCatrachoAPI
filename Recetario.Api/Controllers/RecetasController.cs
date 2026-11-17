using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Recetario.Application;

namespace Recetario.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecetasController : ControllerBase
{
    private readonly IRecetaService _service;

    public RecetasController(IRecetaService service) => _service = service;

    // GET: /api/Recetas
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<RecetaResponseDto>>> GetAll(
        [FromQuery] string? search, [FromQuery] int? categoriaId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return Ok(await _service.GetAllAsync(search, categoriaId, page, pageSize));
    }

    // GET: /api/Recetas/1
    [HttpGet("{id:int}", Name = "GetRecetaById")]
    public async Task<IActionResult> GetById(int id)
    {
        var receta = await _service.GetByIdAsync(id);
        return Ok(new
        {
            mensaje = $"Mostrando los detalles de la receta #{id}: {receta.Nombre}.",
            receta
        });
    }

    // POST: /api/Recetas
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRecetaDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtRoute("GetRecetaById", new { id = result.Id }, new
        {
            mensaje = $"La receta #{result.Id} ('{result.Nombre}') fue creada correctamente.",
            receta = result
        });
    }

    // PUT: /api/Recetas/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRecetaDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return Ok(new { mensaje = $"La receta #{id} fue actualizada correctamente." });
    }

    // DELETE: /api/Recetas/1 (Elimina la receta, su imagen y su video del disco)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return Ok(new { mensaje = $"La receta #{id} y sus archivos multimedia asociados fueron eliminados del servidor." });
    }

    // =========================================================================
    // ENDPOINTS DE IMAGEN EN BASE64
    // =========================================================================

    // PUT: /api/Recetas/1/imagen-base64
    [HttpPut("{id:int}/imagen-base64")]
    public async Task<IActionResult> SubirImagenBase64(int id, [FromBody] ImagenBase64Dto dto)
    {
        await _service.SubirImagenBase64Async(id, dto);
        return Ok(new
        {
            mensaje = $"La fotografía de la receta #{id} se decodificó y guardó correctamente en el servidor."
        });
    }

    // GET: /api/Recetas/1/imagen
    [HttpGet("{id:int}/imagen")]
    public async Task<IActionResult> ObtenerImagen(int id)
    {
        var file = await _service.ObtenerImagenAsync(id);
        return PhysicalFile(file.FullPath, file.ContentType);
    }

    // DELETE: /api/Recetas/1/imagen
    [HttpDelete("{id:int}/imagen")]
    public async Task<IActionResult> EliminarImagen(int id)
    {
        await _service.EliminarImagenAsync(id);
        return Ok(new
        {
            mensaje = $"La fotografía de la receta #{id} fue eliminada correctamente."
        });
    }

    // =========================================================================
    // ENDPOINTS DE VIDEO MP4 CON SOPORTE DE RANGOS
    // =========================================================================

    // PUT: /api/Recetas/1/video
    [HttpPut("{id:int}/video")]
    [RequestSizeLimit(35 * 1024 * 1024)]
    public async Task<IActionResult> SubirVideo(int id, IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
        {
            return BadRequest(new
            {
                mensaje = "No se puede subir el video porque no se adjuntó ningún archivo en el campo 'archivo'."
            });
        }

        using var stream = archivo.OpenReadStream();
        await _service.SubirVideoAsync(id, stream, archivo.FileName, archivo.ContentType, archivo.Length);

        return Ok(new
        {
            mensaje = $"El video MP4 de la receta #{id} se guardó correctamente en el servidor."
        });
    }

    // GET: /api/Recetas/1/video
    [HttpGet("{id:int}/video")]
    public async Task<IActionResult> ObtenerVideo(int id)
    {
        var file = await _service.ObtenerVideoAsync(id);
        return PhysicalFile(file.FullPath, file.ContentType, enableRangeProcessing: true);
    }
}
