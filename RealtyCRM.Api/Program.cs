using RealtyCRM.Api.Interfaces;
using RealtyCRM.Api.Models;
using RealtyCRM.Api.Repositories;
using RealtyCRM.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Внедрение зависимостей для PostgreSQL инфраструктуры
builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddScoped<IUserRepository, SqlUserRepository>();
builder.Services.AddScoped<IPropertyRepository, SqlPropertyRepository>();
builder.Services.AddScoped<IBookingRepository, SqlBookingRepository>();
builder.Services.AddScoped<IDealRepository, SqlDealRepository>();

// Внедрение бизнес-сервисов
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IDealService, DealService>();

var app = builder.Build();

// Инициализация базы данных при старте
try
{
    await DbInitializer.InitializeAsync(app.Configuration);
    Console.WriteLine("База данных успешно проинициализирована.");
}
catch (Exception ex)
{
    Console.WriteLine($"Критическая ошибка при инициализации БД: {ex.Message}");
}

// Настройка HTTP конвейера запросов
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RealtyCRM API V1");
});

app.UseAuthorization();
app.MapControllers();

app.Run();
