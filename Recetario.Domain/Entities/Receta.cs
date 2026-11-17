namespace Recetario.Domain.Entities;

public class Receta
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Ingredientes { get; set; } = string.Empty;
    public string Pasos { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public string? ImagenPath { get; set; }
    public string? VideoPath { get; set; }
}
