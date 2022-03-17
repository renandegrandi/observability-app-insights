using Application.Outputs.V1;

namespace HostedService.Inputs
{
    public class OrderCreateCommandInput
    {
        public OrderOutput Order { get; set; }
    }
}
