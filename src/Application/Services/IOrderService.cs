using Application.Inputs.V1;
using Application.Outputs.V1;

namespace Application.Services
{
    public interface IOrderService
    {
        Task<Guid> CreateAsync(OrderCreateInput orderInput, CancellationToken cancellationToken);

        Task<bool?> UpdateAsync(OrderUpdateInput orderInput, CancellationToken cancellationToken);

        Task<OrderOutput?> GetAsync(Guid id, CancellationToken cancellationToken);

        Task<IEnumerable<OrderOutput>> GetAsync(CancellationToken cancellationToken);
    }
}
