using Microsoft.EntityFrameworkCore;
using Recetario.Domain.Entities;
namespace Recetario.Infrastructure.Persistence;
public class ApplicationDbContext : DbContext {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<Receta> Recetas => Set<Receta>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Categoria>(entity => {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
        });
        modelBuilder.Entity<Receta>(entity => {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Nombre).IsRequired().HasMaxLength(100);
            entity.HasIndex(r => r.Nombre).IsUnique();
            entity.Property(r => r.Ingredientes).IsRequired().HasMaxLength(2000);
            entity.Property(r => r.Pasos).IsRequired().HasMaxLength(4000);
        });
        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nombre = "Desayuno" },
            new Categoria { Id = 2, Nombre = "Almuerzo" },
            new Categoria { Id = 3, Nombre = "Cena" },
            new Categoria { Id = 4, Nombre = "Postre" },
            new Categoria { Id = 5, Nombre = "Bebida" },
            new Categoria { Id = 6, Nombre = "Antojito" }
        );
    }
}
