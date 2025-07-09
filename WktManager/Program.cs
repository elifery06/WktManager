using Microsoft.EntityFrameworkCore;
using WktManager.Data;
using WktManager.Repositories;
using NetTopologySuite;
using NetTopologySuite.IO.Converters;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// DbContext'i NetTopologySuite ile sadece bir kere konfigure et
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
        npgsqlOptions.UseNetTopologySuite()));

// Servislerin eklenmesi
builder.Services.AddScoped<IWktCoordinateService, WktCoordinateService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // GeoJSON deste�i ve di�er JSON ayarlar�
        options.JsonSerializerOptions.NumberHandling =
            System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals;
        options.JsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory());
    });

// Swagger ayarlar�
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS politikas� ekle
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
