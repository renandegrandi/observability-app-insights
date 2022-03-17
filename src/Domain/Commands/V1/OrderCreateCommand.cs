using Domain.Entities;

namespace Domain.Commands.V1
{
    public class OrderCreateCommand
    {
        public OrderCreateCommand() { } 

        public OrderCreateCommand(Order order) : this()
        {
            Order = order;
        }

        public Order Order { get; private set; }
    }
}
