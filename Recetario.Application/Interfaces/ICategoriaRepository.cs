using Recetario.Domain.Entities;
namespace Recetario.Application.Interfaces;
public interface ICategoriaRepository { Task<IEnumerable<Categoria>> GetAllAsync(); }
