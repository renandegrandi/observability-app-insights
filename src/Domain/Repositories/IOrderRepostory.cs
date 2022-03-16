using Domain.Commands.V1;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IOrderRepostory
    {
        Task<Guid> CreateAsync(Order order, CancellationToken cancellationToken);

        Task<Guid> UpdateAsync(Order order, CancellationToken cancellationToken);

        Task<Order> GetAsync(Guid id, CancellationToken cancellationToken);

        Task SendCommandAsync(OrderCreateCommand command, CancellationToken cancellation);
    }
}