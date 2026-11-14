using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Recetario.Domain.Entities;

namespace Recetario.Application;

public record CreateRecetaDto(
    [Required(ErrorMessage = "El nombre de la receta es obligatorio.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
    string Nombre,

    [Required(ErrorMessage = "Los ingredientes son obligatorios.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Los ingredientes deben tener entre 10 y 2000 caracteres.")]
    string Ingredientes,

    [Required(ErrorMessage = "Los pasos de preparación son obligatorios.")]
    [StringLength(4000, MinimumLength = 10, ErrorMessage = "Los pasos deben tener entre 10 y 4000 caracteres.")]
    string Pasos,

    [Required(ErrorMessage = "La categoría es obligatoria.")]
    [Range(1, 6, ErrorMessage = "Debe seleccionar una categoría válida.")]
    int CategoriaId
);

public record UpdateRecetaDto(
    [Required(ErrorMessage = "El nombre de la receta es obligatorio.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
    string Nombre,

    [Required(ErrorMessage = "Los ingredientes son obligatorios.")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Los ingredientes deben tener entre 10 y 2000 caracteres.")]
    string Ingredientes,

    [Required(ErrorMessage = "Los pasos de preparación son obligatorios.")]
    [StringLength(4000, MinimumLength = 10, ErrorMessage = "Los pasos deben tener entre 10 y 4000 caracteres.")]
    string Pasos,

    [Required(ErrorMessage = "La categoría es obligatoria.")]
    [Range(1, 6, ErrorMessage = "Debe seleccionar una categoría válida.")]
    int CategoriaId
);

public record RecetaResponseDto(
    Guid Id,
    string Nombre,
    string Ingredientes,
    string Pasos,
    int CategoriaId
);

public record PagedResultDto<T>(
    IEnumerable<T> Items,
    int Page,
    int PageSize,
    int TotalItems
);
