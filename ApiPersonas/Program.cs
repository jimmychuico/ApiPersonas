using ApiPersonas.Context;
using ApiPersonas.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// cadena de conexión a la base de datos SQL Server local
//var connectionString = builder.Configuration.GetConnectionString("Connection");
//builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
//;

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? $"Server={Environment.GetEnvironmentVariable("DB_HOST")};" +
       $"Database=PersonasDb;" +
       $"User Id=sa;" +
       $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
       $"TrustServerCertificate=True;";


builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString))

builder.Services.AddScoped<IPersonaRepository, PersonaRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});



var app = builder.Build();

app.UseCors(c => c.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// para ejecutar  las migraciones pendientes al iniciar la aplicación
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}


app.Run();
