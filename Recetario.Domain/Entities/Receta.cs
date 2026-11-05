using Recetario.Domain.Enums;
namespace Recetario.Domain.Entities;
public class Receta {
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
    public Dificultad Dificultad { get; set; }
    public int TiempoPreparacionMin { get; set; }
    public int Porciones { get; set; }
    public string Ingredientes { get; set; } = string.Empty;
    public string Pasos { get; set; } = string.Empty;
    public string? ImagenRuta { get; set; }
    public string? VideoRuta { get; set; }
    public DateTime FechaCreacion { get; set; }
}
