using System.IO;
using System.Threading.Tasks;

namespace Recetario.Application;

public interface IRecetaService
{
    Task<PagedResultDto<RecetaResponseDto>> GetAllAsync(string? search, int? categoriaId, int page, int pageSize);
    Task<RecetaResponseDto> GetByIdAsync(int id);
    Task<RecetaResponseDto> CreateAsync(CreateRecetaDto dto);
    Task UpdateAsync(int id, UpdateRecetaDto dto);
    Task DeleteAsync(int id);

    Task SubirImagenBase64Async(int id, ImagenBase64Dto dto);
    Task<FileDownloadDto> ObtenerImagenAsync(int id);
    Task EliminarImagenAsync(int id);
    Task SubirVideoAsync(int id, Stream videoStream, string fileName, string contentType, long length);
    Task<FileDownloadDto> ObtenerVideoAsync(int id);
}
