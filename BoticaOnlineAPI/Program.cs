using Microsoft.EntityFrameworkCore;
using BoticaOnlineAPI.Data;
using BoticaOnlineAPI.Middleware;
using BoticaOnlineAPI.Helpers;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Configurar servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Conexión a MySQL
builder.Services.AddDbContext<BoticaDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 34))));

var app = builder.Build();

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ErrorHandlingMiddleware>();

// Usar CORS
app.UseCors("AllowAll");
app.MapGet("/", () => Results.Redirect("/index.html")).ExcludeFromDescription();

// Habilitar el servicio de archivos estáticos
    app.UseStaticFiles(); // Esto habilita el servicio de archivos desde wwwroot
    app.UseDefaultFiles(); // Permite que index.html se sirva por defecto
app.UseAuthorization();
app.MapControllers();

app.Run();