using Itau.AutoInvest.Infrastructure;
using Itau.AutoInvest.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "Itau AutoInvest API",
        Version = "v1",
        Description = "Sistema de Compra Programada de Ações - Itaú Corretora"
    });
});

builder.Services.AddApplicationCoreLogic(
    builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Itau AutoInvest API v1");
        c.RoutePrefix = string.Empty; // Define o Swagger como página inicial
    });
}

// O Middleware de Exceção deve ser um dos primeiros no pipeline
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
