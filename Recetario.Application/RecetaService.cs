using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Recetario.Domain.Entities;

namespace Recetario.Application;

public class RecetaService : IRecetaService
{
    private readonly IRecetaRepository _repository;

    public RecetaService(IRecetaRepository repository) => _repository = repository;

    public async Task<PagedResultDto<RecetaResponseDto>> GetAllAsync(string? search, int? categoriaId, int page, int pageSize)
    {
        var query = _repository.GetQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(r => r.Nombre.Contains(search) || 
                                     r.Ingredientes.Contains(search) || 
                                     r.Pasos.Contains(search));
        }

        if (categoriaId.HasValue)
        {
            query = query.Where(r => r.CategoriaId == categoriaId.Value);
        }

        int totalItems = query.Count();

        var items = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new RecetaResponseDto(r.Id, r.Nombre, r.Ingredientes, r.Pasos, r.CategoriaId))
            .ToList();

        return new PagedResultDto<RecetaResponseDto>(items, page, pageSize, totalItems);
    }

    public async Task<RecetaResponseDto> GetByIdAsync(Guid id)
    {
        var r = await _repository.GetByIdAsync(id) ?? throw new NotFoundException($"La receta con ID {id} no existe.");
        return new RecetaResponseDto(r.Id, r.Nombre, r.Ingredientes, r.Pasos, r.CategoriaId);
    }

    public async Task<RecetaResponseDto> CreateAsync(CreateRecetaDto dto)
    {
        if (await _repository.ExistsByNameAsync(dto.Nombre))
        {
            throw new ConflictException($"Ya existe una receta registrada con el nombre '{dto.Nombre}'.");
        }

        var nuevaReceta = new Receta
        {
            Id = Guid.NewGuid(),
            Nombre = dto.Nombre,
            Ingredientes = dto.Ingredientes,
            Pasos = dto.Pasos,
            CategoriaId = dto.CategoriaId
        };

        await _repository.AddAsync(nuevaReceta);
        return new RecetaResponseDto(nuevaReceta.Id, nuevaReceta.Nombre, nuevaReceta.Ingredientes, nuevaReceta.Pasos, nuevaReceta.CategoriaId);
    }

    public async Task UpdateAsync(Guid id, UpdateRecetaDto dto)
    {
        var receta = await _repository.GetByIdAsync(id) ?? throw new NotFoundException($"La receta con ID {id} no existe.");

        if (await _repository.ExistsByNameWithDifferentIdAsync(id, dto.Nombre))
        {
            throw new ConflictException($"Ya existe otra receta con el nombre '{dto.Nombre}'.");
        }

        receta.Nombre = dto.Nombre;
        receta.Ingredientes = dto.Ingredientes;
        receta.Pasos = dto.Pasos;
        receta.CategoriaId = dto.CategoriaId;

        await _repository.UpdateAsync(receta);
    }

    public async Task DeleteAsync(Guid id)
    {
        var receta = await _repository.GetByIdAsync(id) ?? throw new NotFoundException($"La receta con ID {id} no existe.");
        await _repository.DeleteAsync(receta);
    }
}
