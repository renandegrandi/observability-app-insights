using Application.Inputs.V1;
using Application.Services;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly ILogger<PedidoController> _logger;
        private readonly TelemetryClient _telemetryClient;
        private readonly IOrderService _orderService;

        public PedidoController(ILogger<PedidoController> logger,
            TelemetryClient telemetryClient,
            IOrderService orderService)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] OrderCreateInput input)  
        {
            try
            {
                var cancellationToken = HttpContext.RequestAborted;

                var result = await _orderService.CreateAsync(input, cancellationToken);

                return Created($"api/pedidos/{result}", null);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync([FromRoute(Name = "id")]Guid id)
        {
            try
            {
                var cancellationToken = HttpContext.RequestAborted;

                var result = await _orderService.GetAsync(id, cancellationToken);

                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex);
                return StatusCode(500);
            }
        }
    }
}
