
using HostedService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

await new HostBuilder()
    .UseEnvironment(env)
    .ConfigureAppConfiguration((hostContext, config) =>
    {
        var environment = hostContext.HostingEnvironment.EnvironmentName;

        config.AddUserSecrets<Program>(true)
            .AddJsonFile($"appsettings.json", false, true)
            .AddJsonFile($"appsettings.{environment}.json", true, true)
            .AddEnvironmentVariables();
    })
    .ConfigureLogging((hostContext, options) => 
    {
        var loggingSection = hostContext
            .Configuration
            .GetSection("Logging");

        options.ClearProviders()
            .AddConsole()
            .AddApplicationInsights()
            .AddConfiguration(loggingSection);
    })
    .ConfigureServices((hostContext, service) =>
    {
        var configuration = hostContext.Configuration;

        service.AddHostedService<AppHostedService>();

        service.AddOptions()
            .AddApplicationInsightsTelemetryWorkerService(configuration);
    })
    .RunConsoleAsync()
    .ConfigureAwait(false);