using Api.Context;
using Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registrar servicio para la conexión a la base de datos
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(connectionString)
);

// Agregar servicios al contenedor.
builder.Services.AddControllers();

// Aprende más sobre cómo configurar Swagger/OpenAPI en: https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Realizar migraciones de base de datos al iniciar la aplicación
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        // Aplicar migraciones pendientes al iniciar la aplicación
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Manejar cualquier error que pueda ocurrir durante la migración de la base de datos
        // Aquí puedes registrar el error en un registro, mostrar un mensaje de error, etc.
        Console.WriteLine("Error occurred while migrating the database: " + ex.Message);
    }
}

app.Run();
