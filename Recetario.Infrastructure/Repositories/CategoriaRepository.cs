using Microsoft.EntityFrameworkCore;
using Recetario.Application.Interfaces;
using Recetario.Domain.Entities;
using Recetario.Infrastructure.Persistence;
namespace Recetario.Infrastructure.Repositories;
public class CategoriaRepository : ICategoriaRepository {
    private readonly ApplicationDbContext _context;
    public CategoriaRepository(ApplicationDbContext context) { _context = context; }
    public async Task<IEnumerable<Categoria>> GetAllAsync() { return await _context.Categorias.ToListAsync(); }
}
