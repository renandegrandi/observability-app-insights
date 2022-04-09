using Application.Inputs.V1;
using Application.Services;
using Azure.Messaging.ServiceBus;
using HostedService.Inputs;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Azure;
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
        private readonly IOrderService _orderService;
        private readonly ILogger<AppHostedService> _logger;

        public AppHostedService(IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
             TelemetryClient telemetryClient,
             IOrderService orderService,
             ILogger<AppHostedService> logger)
        {
            _orderService = orderService;
            _serviceBusClient = serviceBusClientFactory.CreateClient("ExemploSB");
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
            _telemetryClient.TrackException(arg.Exception);

            return Task.CompletedTask;
        }

        private async Task Processor_ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            var message = arg.Message;

            var body = message.Body.ToString();

            if (string.IsNullOrEmpty(body)) return;

            var cancellationToken = arg.CancellationToken;

            var activity = new Activity("AppHostedService:ProcessMessage");

            message.ApplicationProperties.TryGetValue("Diagnostic-Id", out var objectId);

            var diagnosticId = (objectId??"").ToString();

            if(!string.IsNullOrEmpty(diagnosticId))
                activity.SetParentId(diagnosticId);

            using (var telemetry = _telemetryClient.StartOperation<RequestTelemetry>(activity))
            {
                //telemetry.Telemetry.Name = activity.OperationName;
                //telemetry.Telemetry.Context.Operation.Name = activity.OperationName;

                try
                {
                    var command = JsonSerializer.Deserialize<OrderCreateCommandInput>(body);

                    if (command == null) return;

                    var order = command.Order;

                    var modifiedOrder = new OrderUpdateInput
                    {
                        Id = order.Id,
                        Total = 30.00m,
                        Status = Domain.Entities.OrderStatus.Finished
                    };

                    var result = await _orderService.UpdateAsync(modifiedOrder, cancellationToken);

                    if (result == null) 
                        throw new Exception("Order não foi encontrada");

                    if (!result.Value) 
                        throw new Exception("Falha na atualização da order");
                    
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
