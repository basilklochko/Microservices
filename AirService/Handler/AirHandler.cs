using AirService.Storage;
using Common.Helper;
using Common.Implementation;
using Common.Interface;
using Kafka.Interface;
using Microsoft.AspNetCore.SignalR;
using Model;
using System.Text.Json;

namespace AirService.Handler
{
    public class AirHandler : IHandler 
    {
        private readonly string _topic;
        private readonly IPublisher _messageProducer;
        private readonly IHubContext<AirHub, IAirHub> _hubContext;

        public AirHandler(IConfiguration configuration, IPublisher messageProducer, IHubContext<AirHub, IAirHub> hubContext)
        {
            _topic = configuration.GetSection("Kafka").GetSection("OrderTopic").Value;

            _messageProducer = messageProducer;
            _hubContext = hubContext;
        }

        public async Task Reserve(string orderId)
        {
            await LatencyHelper.Delay();

            var index = new Random().Next(-1, Db.Airs.Length - 1);
            var _air = string.Empty;

            if (index > 0)
            {
                _air = Db.Airs[index];
            }

            var order = new Orderable()
            {
                Type = "Air",
                OrderId = orderId,
                Name = _air,
                Status = string.IsNullOrEmpty(_air) ? OrderStatus.Failed : OrderStatus.Pending,
            };

            await _messageProducer.Publish(_topic, order);

            Db.Orders[orderId] = order;

            await _hubContext.Clients.All.Added(order);
        }

        public async Task Cancel(string data)
        {
            await LatencyHelper.Delay();

            var orderId = JsonSerializer.Deserialize<string>(data);

            Db.Orders.TryGetValue(orderId, out var order);

            if (order is not null && order.Status != OrderStatus.Failed)
            {
                order.UpdatedAt = DateTime.Now;
                order.Status = OrderStatus.Canceled;

                await _hubContext.Clients.All.Canceled(order);
            }
        }

        public async Task Confirm(string data)
        {
            await LatencyHelper.Delay();

            var orderId = JsonSerializer.Deserialize<string>(data);

            Db.Orders.TryGetValue(orderId, out var order);

            if (order is not null)
            {
                order.UpdatedAt = DateTime.Now;
                order.Status = OrderStatus.Confirmed;

                await _hubContext.Clients.All.Confirmed(order);
            }
        }

        public async Task Fail(string data)
        {
            await LatencyHelper.Delay();

            var orderId = JsonSerializer.Deserialize<string>(data);

            Db.Orders.TryGetValue(orderId, out var order);

            if (order is not null)
            {
                order.UpdatedAt = DateTime.Now;
                order.Status = OrderStatus.Failed;

                await _hubContext.Clients.All.Failed(order);
            }
        }
    }
}
