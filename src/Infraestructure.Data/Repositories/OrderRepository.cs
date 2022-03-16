using Domain.Commands.V1;
using Domain.Entities;
using Domain.Repositories;

namespace Infraestructure.Data.Repositories
{
    sealed class OrderRepository : IOrderRepostory
    {



        public Task<Guid> CreateAsync(Order order, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SendCommandAsync(OrderCreateCommand command, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> UpdateAsync(Order order, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
