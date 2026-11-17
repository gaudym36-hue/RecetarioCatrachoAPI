using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Recetario.Api;
using Recetario.Application;
using Recetario.Application.Interfaces;
using Recetario.Infrastructure.Repositories;
using Recetario.Infrastructure.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Permitir archivos de hasta 35 MB para que los videos MP4 de 30 MB pasen al controlador
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 35 * 1024 * 1024;
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 35 * 1024 * 1024;
});

builder.Services.AddDbContext<Recetario.Infrastructure.Persistence.ApplicationDbContext>();

builder.Services.AddControllers();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IRecetaRepository, RecetaRepository>();
builder.Services.AddScoped<IRecetaService, RecetaService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Recetario.Infrastructure.Persistence.ApplicationDbContext>();
    db.Database.EnsureCreated();
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthorization();
app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();

// Redirigir automáticamente la raíz a la documentación visual con todos los endpoints
app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();

app.Run();
