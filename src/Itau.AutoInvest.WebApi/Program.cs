using Itau.AutoInvest.Infrastructure;
using Itau.AutoInvest.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Itau.AutoInvest.WebApi.Middlewares;
using OpenTelemetry.Metrics;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

var lokiHttpUrl = builder.Configuration["Loki:ConnectionString"] ??
                  throw new ArgumentException("Loki:ConnectionString is null");
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.GrafanaLoki(lokiHttpUrl, new [] { new LokiLabel { Key = "job", Value = "itau-autoinvest-api" } })
    .CreateLogger();

builder.Host.UseSerilog();

// Configuração OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddMeter("Microsoft.EntityFrameworkCore")
            .AddPrometheusExporter();
    });

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

builder.Services.AddApplicationCoreLogic(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    context.Database.Migrate();
}

app.UseOpenTelemetryPrometheusScrapingEndpoint();


app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Itau AutoInvest API v1"); 
    c.RoutePrefix = string.Empty;
});


app.UseMiddleware<RequestIdMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.MapControllers();

app.Run();
public partial class Program { }
