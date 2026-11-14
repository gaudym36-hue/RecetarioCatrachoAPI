using System;

namespace Recetario.Domain.Entities;

public class Receta
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Ingredientes { get; set; } = string.Empty;
    public string Pasos { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
}
