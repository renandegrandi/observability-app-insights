using Application;
using Infraestructure.Data;
using Microsoft.ApplicationInsights.DependencyCollector;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

var logging = builder.Logging;
var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddApplicationInsightsTelemetry(configuration)
    .ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) =>
    {
        module.EnableSqlCommandTextInstrumentation = true;
    });

services.AddInfraestructure(configuration)
    .AddApplication();

logging.ClearProviders()
    .AddConsole()
    .AddApplicationInsights()
    .AddConfiguration(configuration);

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
