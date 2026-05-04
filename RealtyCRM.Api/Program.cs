using Microsoft.OpenApi.Models;
using RealtyCRM.Api.Interfaces;
using RealtyCRM.Api.Models;
using RealtyCRM.Api.Repositories;
using RealtyCRM.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RealtyCRM API",
        Version = "v1",
        Description = "API для управления агентством недвижимости"
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Dependency Injection
builder.Services.AddSingleton<IRepository<Property>, InMemoryRepository<Property>>();
builder.Services.AddSingleton<IRepository<Deal>, InMemoryRepository<Deal>>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IDealService, DealService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
