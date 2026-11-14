using Recetario.Application.Interfaces;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Recetario.Application;
using Recetario.Infrastructure.Repositories;
using Recetario.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Recetario.Infrastructure.Persistence.ApplicationDbContext>();

builder.Services.AddControllers();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IRecetaRepository, RecetaRepository>();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IRecetaRepository, RecetaRepository>();
builder.Services.AddScoped<IRecetaService, RecetaService>();

var app = builder.Build();
using (var scope = app.Services.CreateScope()) { var db = scope.ServiceProvider.GetRequiredService<Recetario.Infrastructure.Persistence.ApplicationDbContext>(); db.Database.EnsureCreated(); }

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthorization();
app.MapOpenApi();
app.MapScalarApiReference();
app.MapControllers();

app.Run();





