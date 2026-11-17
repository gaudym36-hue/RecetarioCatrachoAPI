using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Recetario.Application;
using Recetario.Domain.Entities;
using Recetario.Infrastructure.Persistence;

namespace Recetario.Infrastructure.Repositories;

public class RecetaRepository : IRecetaRepository
{
    private readonly ApplicationDbContext _context;
    public RecetaRepository(ApplicationDbContext context) => _context = context;
    public IQueryable<Receta> GetQueryable() => _context.Set<Receta>().AsNoTracking();
    public async Task<Receta?> GetByIdAsync(int id) => await _context.Set<Receta>().FindAsync(id);
    public async Task<bool> ExistsByNameAsync(string nombre) => await _context.Set<Receta>().AnyAsync(r => r.Nombre.ToLower() == nombre.ToLower());
    public async Task<bool> ExistsByNameWithDifferentIdAsync(int id, string nombre) => await _context.Set<Receta>().AnyAsync(r => r.Id != id && r.Nombre.ToLower() == nombre.ToLower());
    public async Task AddAsync(Receta receta) { await _context.Set<Receta>().AddAsync(receta); await _context.SaveChangesAsync(); }
    public async Task UpdateAsync(Receta receta) { _context.Set<Receta>().Update(receta); await _context.SaveChangesAsync(); }
    public async Task DeleteAsync(Receta receta) { _context.Set<Receta>().Remove(receta); await _context.SaveChangesAsync(); }
}
