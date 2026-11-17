using System;
using System.IO;
using System.Threading.Tasks;
using Recetario.Application.Interfaces;

namespace Recetario.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public LocalFileStorageService()
    {
        _basePath = Directory.GetCurrentDirectory();
    }

    public async Task<string> SaveBytesAsync(byte[] content, string folder, string extension)
    {
        var directory = Path.Combine(_basePath, "Uploads", folder);
        Directory.CreateDirectory(directory);

        var fileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(directory, fileName);

        await File.WriteAllBytesAsync(fullPath, content);
        return Path.Combine("Uploads", folder, fileName);
    }

    public async Task<string> SaveStreamAsync(Stream stream, string folder, string extension)
    {
        var directory = Path.Combine(_basePath, "Uploads", folder);
        Directory.CreateDirectory(directory);

        var fileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(directory, fileName);

        using var fileStream = new FileStream(fullPath, FileMode.Create);
        await stream.CopyToAsync(fileStream);

        return Path.Combine("Uploads", folder, fileName);
    }

    public void DeleteFile(string? relativePath)
    {
        if (string.IsNullOrEmpty(relativePath)) return;
        var fullPath = GetFullPath(relativePath);
        if (File.Exists(fullPath)) File.Delete(fullPath);
    }

    public string GetFullPath(string relativePath) => Path.Combine(_basePath, relativePath);

    public bool FileExists(string relativePath) => File.Exists(GetFullPath(relativePath));
}
