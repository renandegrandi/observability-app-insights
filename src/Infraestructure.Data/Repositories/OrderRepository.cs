using Azure.Messaging.ServiceBus;
using Domain.Commands.V1;
using Domain.Entities;
using Domain.Repositories;
using Infraestructure.Data.Contexts.MongoDB;
using Infraestructure.Data.Contexts.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using MongoDB.Driver;
using System.Text.Json;

namespace Infraestructure.Data.Repositories
{
    sealed class OrderRepository : IOrderRepostory
    {
        private const string Queue = "order-create-command";

        private readonly SqlContext _sqlContext;
        private readonly ServiceBusClient _messageContext;
        private readonly IMongoCollection<Order> _collection;

        public OrderRepository(SqlContext sqlContext,
            IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
            IMongoContext mongoContext)
        {
            _sqlContext = sqlContext;
            _collection = mongoContext.Database.GetCollection<Order>("Orders");
            _messageContext = serviceBusClientFactory.CreateClient("ExemploSB");
        }


        public async Task CreateAsync(Order order, CancellationToken cancellationToken)
        {
            //simulate integration with mongodb.
            _sqlContext.Add(order);
            await _collection.InsertOneAsync(order, null, cancellationToken);
            await _sqlContext.SaveChangesAsync(cancellationToken);
        }

        public Task<Order?> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            return _sqlContext
                .Order
                .FirstOrDefaultAsync((order) => order.Id == id, cancellationToken);

        }

        public Task<List<Order>> GetAsync(CancellationToken cancellationToken)
        {
            return _sqlContext
                .Order
                .ToListAsync(cancellationToken);
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
