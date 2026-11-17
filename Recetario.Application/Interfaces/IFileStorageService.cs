namespace Recetario.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveBytesAsync(byte[] content, string folder, string extension);
    Task<string> SaveStreamAsync(Stream stream, string folder, string extension);
    void DeleteFile(string? relativePath);
    string GetFullPath(string relativePath);
    bool FileExists(string relativePath);
}
