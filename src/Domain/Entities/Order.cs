namespace Domain.Entities
{
    public class Order
    {
        private Order () 
        {
            Id = Guid.NewGuid ();
            Status = OrderStatus.Created;
        }

        public Order(decimal total): this()
        {
            Total = total;
        }

        public Guid Id { get; private set; }
        public OrderStatus Status { get; private set; }
        public decimal Total { get; private set; }

        public Order ChangeStatus(OrderStatus status) 
        {
            Status = status;
            return this;
        }

        public Order ChangeTotal(decimal total) 
        {
            Total = total;
            return this;
        }
    }
}
