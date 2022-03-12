using Azure.Messaging.ServiceBus;
using HostedService.Commands.V1;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace HostedService
{
    internal class AppHostedService : IHostedService
    {
        private const string QueueName = "order-create-command";
        private readonly ServiceBusClient _serviceBusClient;
        private readonly TelemetryClient _telemetryClient;
        private readonly ILogger<AppHostedService> _logger;

        public AppHostedService(IConfiguration configuration,
             TelemetryClient telemetryClient,
             ILogger<AppHostedService> logger)
        {
            var sbConnectionString = configuration["ServiceBus:ConnectionString"];

            _serviceBusClient = new ServiceBusClient(sbConnectionString);
            _telemetryClient = telemetryClient;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var processor = _serviceBusClient.CreateProcessor(QueueName);

            processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
            processor.ProcessErrorAsync += Processor_ProcessErrorAsync;

            await processor
                    .StartProcessingAsync(cancellationToken)
                    .ConfigureAwait(false);
        }

        private Task Processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            return Task.CompletedTask;
        }

        private async Task Processor_ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            var body = arg.Message.Body.ToString();
            var cancellationToken = arg.CancellationToken;

            using (var telemetry = _telemetryClient.StartOperation<RequestTelemetry>("AppHostedService:ProcessMessage"))
            {
                try
                {
                    var command = JsonSerializer.Deserialize<PedidoCreateCommand>(body);

                    _logger.LogInformation("Pedido: {}, começando processamento!", command.Pedido.Id);

                    _logger.LogInformation("Commando executado com sucesso para o pedido: {0}", command.Pedido.Id.ToString());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);

                    await arg.DeadLetterMessageAsync(arg.Message, null, cancellationToken)
                        .ConfigureAwait(false);
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _telemetryClient.FlushAsync(cancellationToken);
        }
    }
}
