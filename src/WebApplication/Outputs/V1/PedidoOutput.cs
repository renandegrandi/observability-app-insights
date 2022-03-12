namespace WebApplication.Outputs.V1
{
    public class PedidoOutput
    {
        public PedidoOutput()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
    }
}
