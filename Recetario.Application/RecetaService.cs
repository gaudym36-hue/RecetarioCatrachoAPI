using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Recetario.Application.Interfaces;
using Recetario.Domain.Entities;

namespace Recetario.Application;

public class RecetaService : IRecetaService
{
    private readonly IRecetaRepository _repository;
    private readonly IFileStorageService _fileStorage;

    private const long MaxImagenBytes = 5 * 1024 * 1024;  // 5 MB
    private const long MaxVideoBytes = 30 * 1024 * 1024;  // 30 MB

    public RecetaService(IRecetaRepository repository, IFileStorageService fileStorage)
    {
        _repository = repository;
        _fileStorage = fileStorage;
    }

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

    public async Task<RecetaResponseDto> GetByIdAsync(int id)
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
            Nombre = dto.Nombre,
            Ingredientes = dto.Ingredientes,
            Pasos = dto.Pasos,
            CategoriaId = dto.CategoriaId
        };

        await _repository.AddAsync(nuevaReceta);
        return new RecetaResponseDto(nuevaReceta.Id, nuevaReceta.Nombre, nuevaReceta.Ingredientes, nuevaReceta.Pasos, nuevaReceta.CategoriaId);
    }

    public async Task UpdateAsync(int id, UpdateRecetaDto dto)
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

    public async Task DeleteAsync(int id)
    {
        var receta = await _repository.GetByIdAsync(id) ?? throw new NotFoundException($"La receta con ID {id} no existe.");

        _fileStorage.DeleteFile(receta.ImagenPath);
        _fileStorage.DeleteFile(receta.VideoPath);

        await _repository.DeleteAsync(receta);
    }

    public async Task SubirImagenBase64Async(int id, ImagenBase64Dto dto)
    {
        var receta = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"La receta con ID {id} no existe.");

        if (string.IsNullOrWhiteSpace(dto.ImagenBase64))
            throw new BadRequestException("La cadena Base64 es obligatoria.");

        var base64Data = dto.ImagenBase64.Contains(",")
            ? dto.ImagenBase64.Split(',')[1]
            : dto.ImagenBase64;

        byte[] imageBytes;
        try
        {
            imageBytes = Convert.FromBase64String(base64Data);
        }
        catch (FormatException)
        {
            throw new BadRequestException("El texto enviado no es un Base64 válido.");
        }

        if (imageBytes.Length > MaxImagenBytes)
            throw new PayloadTooLargeException("La imagen supera el límite máximo de 5 MB.");

        var extension = ObtenerExtensionImagenValida(imageBytes)
            ?? throw new UnsupportedMediaTypeException("Formato no permitido. Solo JPG, PNG o WebP.");

        _fileStorage.DeleteFile(receta.ImagenPath);

        receta.ImagenPath = await _fileStorage.SaveBytesAsync(imageBytes, "Imagenes", extension);
        await _repository.UpdateAsync(receta);
    }

    public async Task<FileDownloadDto> ObtenerImagenAsync(int id)
    {
        var receta = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"La receta con ID {id} no existe.");

        if (string.IsNullOrEmpty(receta.ImagenPath) || !_fileStorage.FileExists(receta.ImagenPath))
            throw new NotFoundException("La receta no tiene una imagen registrada.");

        var fullPath = _fileStorage.GetFullPath(receta.ImagenPath);
        var contentType = Path.GetExtension(fullPath).ToLower() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };

        return new FileDownloadDto(fullPath, contentType);
    }

    public async Task EliminarImagenAsync(int id)
    {
        var receta = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"La receta con ID {id} no existe.");

        if (string.IsNullOrEmpty(receta.ImagenPath))
            throw new NotFoundException("La receta no tiene una imagen para eliminar.");

        _fileStorage.DeleteFile(receta.ImagenPath);
        receta.ImagenPath = null;
        await _repository.UpdateAsync(receta);
    }

    public async Task SubirVideoAsync(int id, Stream videoStream, string fileName, string contentType, long length)
    {
        var receta = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"La receta con ID {id} no existe.");

        if (length == 0)
            throw new BadRequestException("El archivo de video está vacío.");

        if (length > MaxVideoBytes)
            throw new PayloadTooLargeException("El video supera el límite máximo de 30 MB.");

        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        if (ext != ".mp4" || contentType != "video/mp4")
            throw new UnsupportedMediaTypeException("Solo se permiten videos en formato MP4.");

        _fileStorage.DeleteFile(receta.VideoPath);

        receta.VideoPath = await _fileStorage.SaveStreamAsync(videoStream, "Videos", ".mp4");
        await _repository.UpdateAsync(receta);
    }

    public async Task<FileDownloadDto> ObtenerVideoAsync(int id)
    {
        var receta = await _repository.GetByIdAsync(id)
            ?? throw new NotFoundException($"La receta con ID {id} no existe.");

        if (string.IsNullOrEmpty(receta.VideoPath) || !_fileStorage.FileExists(receta.VideoPath))
            throw new NotFoundException("La receta no tiene un video registrado.");

        return new FileDownloadDto(_fileStorage.GetFullPath(receta.VideoPath), "video/mp4");
    }

    private static string? ObtenerExtensionImagenValida(byte[] bytes)
    {
        if (bytes.Length < 12) return null;
        if (bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF) return ".jpg";
        if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47) return ".png";
        if (bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46 &&
            bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50) return ".webp";
        return null;
    }
}
