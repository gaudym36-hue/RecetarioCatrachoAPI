using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Recetario.Application.Interfaces;
using Recetario.Infrastructure.Persistence;
using Recetario.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configuración limpia en memoria que salta el bloqueo de usuarios de Windows
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("RecetarioCatrachoDb"));

builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddControllers();
builder.Services.AddOpenApi(options => {
    options.AddDocumentTransformer((document, context, cancellationToken) => {
        document.Info.Title = "🍳 Recetario Catracho -- Sabores de mi Tierra";
        document.Info.Version = "Fase 1";
        document.Info.Description = "Base de la API REST organizada en Arquitectura Limpia.";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Forzar la carga automática de tus 6 datos semilla en memoria
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
    app.MapScalarApiReference(options => {
        options.WithTheme(ScalarTheme.Saturn).WithTitle("Panel de Pruebas").WithSidebar(true);
    });
}

app.UseAuthorization();
app.MapControllers();
app.Run();
