using Microsoft.EntityFrameworkCore;
using Recetario.Domain.Entities;

namespace Recetario.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseInMemoryDatabase("RecetarioDb");
        }
    }

    public DbSet<Receta> Recetas => Set<Receta>();
    public DbSet<Categoria> Categorias => Set<Categoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Receta>().HasData(
            new Receta { Id = 1, Nombre = "Baleadas Sencillas y Especiales", Ingredientes = "Tortillas de harina, frijoles fritos, mantequilla, queso rallado, huevo revuelto, aguacate", Pasos = "Calentar la tortilla de harina, untar frijoles fritos, agregar mantequilla, queso, huevo y aguacate al gusto.", CategoriaId = 1 },
            new Receta { Id = 2, Nombre = "Casamiento Catracho con Huevos", Ingredientes = "Arroz blanco cocido, frijoles rojos cocidos con su caldo, cebolla, chile dulce, huevos, platano maduro", Pasos = "Sofreir cebolla y chile, agregar frijoles y arroz hasta integrar. Servir con huevos y tajadas de maduro.", CategoriaId = 1 },
            new Receta { Id = 3, Nombre = "Sopa de Caracol", Ingredientes = "Caracol limpio, leche de coco, yuca, guineo verde, zanahoria, culantro fino, ajo, cebolla, achiote", Pasos = "Cocinar las verduras en leche de coco con especias y achiote; agregar el caracol los ultimos 5 minutos.", CategoriaId = 2 },
            new Receta { Id = 4, Nombre = "Pollo Chuco con Tajadas", Ingredientes = "Piezas de pollo, guineo verde, repollo picado, salsa de tomate natural, aderezo, encurtido de cebolla", Pasos = "Freir el pollo sazonado y las tajadas de guineo verde. Servir con ensalada de repollo, salsas y encurtido.", CategoriaId = 2 },
            new Receta { Id = 5, Nombre = "Pupusas de Quesillo con Loroco", Ingredientes = "Harina de maiz, quesillo, loroco picado, repollo curtido, salsa de tomate", Pasos = "Amasar la harina con agua, rellenar con quesillo y loroco, dar forma redonda y cocinar en comal hasta dorar.", CategoriaId = 3 },
            new Receta { Id = 6, Nombre = "Enchiladas Hondurenas", Ingredientes = "Tortillas de maiz tostadas, carne molida condimentada, repollo, salsa de tomate, huevo duro, queso rallado", Pasos = "Colocar carne molida sobre la tortilla frita, anadir repollo, salsa, rodaja de huevo duro y queso rallado.", CategoriaId = 3 },
            new Receta { Id = 7, Nombre = "Torrejas en Miel de Rapadura", Ingredientes = "Pan de yema o bolillo, huevos batidos, dulce de rapadura, canela en raja, clavos de olor", Pasos = "Pasar rebanadas de pan por huevo batido y freir. Hervir en miel de rapadura con canela y clavos.", CategoriaId = 4 },
            new Receta { Id = 8, Nombre = "Arroz con Leche Catracho", Ingredientes = "Arroz, leche entera, leche condensada, canela en raja, pasas, canela en polvo", Pasos = "Cocinar el arroz con agua y canela hasta ablandar, agregar las leches y pasas, cocinar hasta espesar.", CategoriaId = 4 },
            new Receta { Id = 9, Nombre = "Horchata de Morro", Ingredientes = "Semilla de morro, arroz, cacahuate, canela, leche o agua, azucar, hielo", Pasos = "Tostar y moler los ingredientes, colar con agua o leche fria, endulzar al gusto y servir con hielo.", CategoriaId = 5 },
            new Receta { Id = 10, Nombre = "Atol de Elote", Ingredientes = "Elotes tiernos desgranados, leche, azucar, canela en raja, pizca de sal", Pasos = "Licuar el elote con leche, colar y cocinar a fuego medio con canela y azucar sin dejar de mover.", CategoriaId = 5 },
            new Receta { Id = 11, Nombre = "Pastelitos de Carne con Arroz", Ingredientes = "Masa de maiz con achiote, carne molida con arroz y papa picada, repollo curtido, salsa", Pasos = "Rellenar discos de masa con el guiso de carne y arroz, cerrar en media luna y freir hasta dorar.", CategoriaId = 6 },
            new Receta { Id = 12, Nombre = "Catrachas de Frijoles con Queso", Ingredientes = "Tortillas de maiz fritas, frijoles rojos licuados y fritos, queso seco rallado", Pasos = "Freir las tortillas hasta tostar, untar frijoles fritos calientes y cubrir con abundante queso rallado.", CategoriaId = 6 }
        );

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
