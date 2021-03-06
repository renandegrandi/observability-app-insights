using Application.Inputs.V1;
using Application.Outputs.V1;
using Domain.Commands.V1;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Application.Services
{
    sealed class OrderService : IOrderService
    {
        private const string OrderKeyCached = "Order.";
        
        private readonly IOrderRepostory _orderRepository;
        private readonly IDistributedCache _distributedCache;
        private readonly TelemetryClient _telemetryClient;

        public OrderService(IOrderRepostory orderRepository,
            IDistributedCache distributedCache,
            TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
            _orderRepository = orderRepository;
            _distributedCache = distributedCache;
        }

        private async Task<string> SetToCacheAsync(Order order, CancellationToken cancellationToken) 
        {
            var orderSerialized = JsonSerializer.Serialize(order);

            var key = $"{OrderKeyCached}{order.Id}";

            using (var operation = _telemetryClient.StartOperation(new DependencyTelemetry
            {
                Type = "DistributedCache",
                Name = "SetStringAsync",
                Data = key
            })) 
                await _distributedCache.SetStringAsync(key, orderSerialized, cancellationToken);
            
            return orderSerialized;
        }

        private void SimulateExceptionWithRandomValue() 
        {
            var random = new Random().Next(100);

            if (random % 2 == 0)
                throw new Exception("Demonstrando uma exception no código!");
        }

        public async Task<Guid> CreateAsync(OrderCreateInput orderInput, CancellationToken cancellationToken)
        {
            SimulateExceptionWithRandomValue();

            var order = new Order(orderInput.Total);

            await _orderRepository.CreateAsync(order, cancellationToken);

            var command = new OrderCreateCommand(order);

            await _orderRepository.SendCommandAsync(command, cancellationToken);

            return order.Id;
        }

        public async Task<bool?> UpdateAsync(OrderUpdateInput orderInput, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetAsync(orderInput.Id, cancellationToken);

            if (order == null) return null;

            order
                .ChangeStatus(orderInput.Status)
                .ChangeTotal(orderInput.Total);

            await _orderRepository.UpdateAsync(order, cancellationToken);

            await SetToCacheAsync(order, cancellationToken);

            return true;
        }

        public async Task<OrderOutput?> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            var orderCached = string.Empty;

            var key = $"{OrderKeyCached}{id}";

            using (var operation = _telemetryClient.StartOperation(new DependencyTelemetry
            {
                Type = "DistributedCache",
                Name = "GetStringAsync",
                Data = key
            }))
                orderCached = await _distributedCache.GetStringAsync(key, cancellationToken);

            if (string.IsNullOrEmpty(orderCached))
            {
                var order = await _orderRepository.GetAsync(id, cancellationToken);

                if (order == null) return null;

                orderCached = await SetToCacheAsync(order, cancellationToken);
            }

            return JsonSerializer.Deserialize<OrderOutput?>(orderCached);       
        }

        public  async Task<IEnumerable<OrderOutput>> GetAsync(CancellationToken cancellationToken)
        {
            var ordersDb = await _orderRepository.GetAsync(cancellationToken);

            if(!ordersDb.Any()) return new List<OrderOutput>();

            var dbSerialized = JsonSerializer.Serialize(ordersDb);

            return JsonSerializer.Deserialize<IEnumerable<OrderOutput>>(dbSerialized);
        }
    }
}
