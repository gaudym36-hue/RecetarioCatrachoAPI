using System;
using System.Threading.Tasks;
using Recetario.Domain.Entities;

namespace Recetario.Application;

public interface IRecetaService
{
    Task<PagedResultDto<RecetaResponseDto>> GetAllAsync(string? search, int? categoriaId, int page, int pageSize);
    Task<RecetaResponseDto> GetByIdAsync(Guid id);
    Task<RecetaResponseDto> CreateAsync(CreateRecetaDto dto);
    Task UpdateAsync(Guid id, UpdateRecetaDto dto);
    Task DeleteAsync(Guid id);
}
