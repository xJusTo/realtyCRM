using RealtyCRM.Api.Interfaces;
using RealtyCRM.Api.Models;
using RealtyCRM.Api.Repositories;
using RealtyCRM.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.AddSingleton<IRepository<Property>, InMemoryRepository<Property>>();
builder.Services.AddSingleton<IRepository<Deal>, InMemoryRepository<Deal>>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IDealService, DealService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RealtyCRM API V1");
});

// app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
