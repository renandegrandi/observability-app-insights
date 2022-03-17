using Domain.Entities;

namespace Application.Outputs.V1
{
    public class OrderOutput
    {
        public Guid Id { get; set; }

        public OrderStatus Status { get; set; }

        public decimal Total { get; set; }
    }
}
