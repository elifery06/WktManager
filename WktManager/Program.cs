using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WktManager.Data;
using WktManager.Repositories;


var builder = WebApplication.CreateBuilder(args);

// DbContext ve PostgreSQL baðlantýsý
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddScoped<IWktCoordinateService, WktCoordinateService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
