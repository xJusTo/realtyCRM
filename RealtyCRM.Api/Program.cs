using RealtyCRM.Api.Interfaces;
using RealtyCRM.Api.Models;
using RealtyCRM.Api.Repositories;
using RealtyCRM.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Dependency Injection
builder.Services.AddSingleton<IRepository<Property>, InMemoryRepository<Property>>();
builder.Services.AddSingleton<IRepository<Deal>, InMemoryRepository<Deal>>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IDealService, DealService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
