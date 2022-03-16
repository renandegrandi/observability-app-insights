using Microsoft.Extensions.Azure;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

var logging = builder.Logging;
var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddApplicationInsightsTelemetry(configuration);

services.AddAzureClients((builder) =>
{
    builder.AddServiceBusClient(configuration["ServiceBus:ConnectionString"])
        .WithName("ExemploSB");
});

logging.ClearProviders()
    .AddConsole()
    .AddApplicationInsights()
    .AddEventSourceLogger()
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
