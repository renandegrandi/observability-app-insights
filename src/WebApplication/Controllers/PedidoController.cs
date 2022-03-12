using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebApplication.Commands.V1;
using WebApplication.Outputs.V1;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private const string Queue = "order-create-command";
        private readonly string _sbConnectionString;
        private ILogger<PedidoController> _logger;

        public PedidoController(IConfiguration configuration,
            ILogger<PedidoController> logger)
        {
            _sbConnectionString = configuration["ServiceBus:ConnectionString"];
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync() 
        {
            var cancellationToken = HttpContext.RequestAborted;

            var sender = new ServiceBusClient(_sbConnectionString)
                .CreateSender(Queue);

            var command = new PedidoCreateCommand
            {
                Pedido = new PedidoOutput()
            };

            var commadSerialized = JsonSerializer.Serialize(command);

            var message = new ServiceBusMessage(commadSerialized);

            await sender.SendMessageAsync(message, cancellationToken)
                .ConfigureAwait(false);

            _logger.LogInformation("Pedido: {0}, enviado com sucesso!", command.Pedido.Id);

            return Created($"api/pedidos/{command.Pedido.Id}", null);
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            await Task.CompletedTask;

            var output = new PedidoOutput();

            return Ok(output);
        }
    }
}
