namespace Application.Outputs.V1
{
    public class OrderOutput
    {
        public OrderOutput()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
    }
}
