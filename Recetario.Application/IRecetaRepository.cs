using System.Linq;
using System.Threading.Tasks;
using Recetario.Domain.Entities;

namespace Recetario.Application;

public interface IRecetaRepository
{
    IQueryable<Receta> GetQueryable();
    Task<Receta?> GetByIdAsync(int id);
    Task<bool> ExistsByNameAsync(string nombre);
    Task<bool> ExistsByNameWithDifferentIdAsync(int id, string nombre);
    Task AddAsync(Receta receta);
    Task UpdateAsync(Receta receta);
    Task DeleteAsync(Receta receta);
}


