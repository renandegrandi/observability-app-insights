using Azure.Messaging.ServiceBus;
using Domain.Commands.V1;
using Domain.Entities;
using Domain.Repositories;
using Infraestructure.Data.Contexts.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using System.Text.Json;

namespace Infraestructure.Data.Repositories
{
    sealed class OrderRepository : IOrderRepostory
    {
        private const string Queue = "order-create-command";

        private readonly SqlContext _sqlContext;
        private readonly ServiceBusClient _messageContext;

        public OrderRepository(SqlContext sqlContext,
            IAzureClientFactory<ServiceBusClient> serviceBusClientFactory)
        {
            _sqlContext = sqlContext;
            _messageContext = serviceBusClientFactory.CreateClient("ExemploSB");
        }


        public Task CreateAsync(Order order, CancellationToken cancellationToken)
        {
            _sqlContext.Add(order);
            return _sqlContext.SaveChangesAsync(cancellationToken);
        }

        public Task<Order?> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            return _sqlContext
                .Order
                .FirstOrDefaultAsync((order) => order.Id == id, cancellationToken);

        }

        public Task UpdateAsync(Order order, CancellationToken cancellationToken)
        {
            _sqlContext.Update(order);
            return _sqlContext.SaveChangesAsync(cancellationToken);
        }

        public Task SendCommandAsync(OrderCreateCommand command, CancellationToken cancellationToken)
        {
            var sender = _messageContext.CreateSender(Queue);

            var commandSerialized = JsonSerializer.Serialize(command);

            var message = new ServiceBusMessage(commandSerialized);

            return sender.SendMessageAsync(message, cancellationToken);
        }
    }
}
