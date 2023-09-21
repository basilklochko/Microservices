using BookingService.Storage;
using Common.Helper;
using Common.Implementation;
using Common.Interface;
using Kafka.Interface;
using Microsoft.AspNetCore.SignalR;
using Model;
using System.Text.Json;

namespace BookingService.Handler
{
    public class BookingHandler : IHandler
    {
        private readonly string _hotelFailTopic;
        private readonly string _airFailTopic;

        private readonly string _hotelCancelTopic;
        private readonly string _airCancelTopic;

        private readonly string _hotelConfirmTopic;
        private readonly string _airConfirmTopic;

        private readonly IPublisher _messageProducer;
        private readonly IHubContext<BookingHub, IBookingHub> _hubContext;

        public BookingHandler(IConfiguration configuration, ITopic topic, IPublisher messageProducer, IHubContext<BookingHub, IBookingHub> hubContext)
        {
            _hotelFailTopic = configuration.GetSection("Kafka").GetSection("HotelFailTopic").Value;
            _airFailTopic = configuration.GetSection("Kafka").GetSection("AirFailTopic").Value;

            _hotelCancelTopic = configuration.GetSection("Kafka").GetSection("HotelCancelTopic").Value;
            _airCancelTopic = configuration.GetSection("Kafka").GetSection("AirCancelTopic").Value;

            _hotelConfirmTopic = configuration.GetSection("Kafka").GetSection("HotelConfirmTopic").Value;
            _airConfirmTopic = configuration.GetSection("Kafka").GetSection("AirConfirmTopic").Value;

            _messageProducer = messageProducer;
            _hubContext = hubContext;
        }

        public Task Cancel(string data)
        {
            throw new NotImplementedException();
        }

        public Task Confirm(string data)
        {
            throw new NotImplementedException();
        }

        public Task Fail(string data)
        {
            throw new NotImplementedException();
        }

        public async Task Reserve(string data)
        {
            await LatencyHelper.Delay();

            var orderable = JsonSerializer.Deserialize<Orderable>(data);

            if (orderable is not null)
            {
                Db.Orders.TryGetValue(orderable.OrderId, out var order);

                if (order is null)
                {
                    Db.Orders[orderable.OrderId] = new Order();
                    order = Db.Orders[orderable.OrderId];
                }

                switch (orderable.Type)
                {
                    case "Hotel":
                        order.Hotel = orderable;
                        
                        if (order.Status == OrderStatus.Pending && orderable.Name == String.Empty)
                        {
                            order.Hotel.UpdatedAt = DateTime.Now;
                            order.Hotel.Status = OrderStatus.Failed;

                            await _messageProducer.Publish(_hotelFailTopic, orderable.OrderId);
                        }
                        break;

                    case "Air":
                        order.Air = orderable;

                        if (order.Status == OrderStatus.Pending && orderable.Name == String.Empty)
                        {
                            order.Air.UpdatedAt = DateTime.Now;
                            order.Air.Status = OrderStatus.Failed;

                            await _messageProducer.Publish(_airFailTopic, orderable.OrderId);
                        }
                        break;

                    default:
                        break;
                }

                switch (order.Status)
                {
                    case OrderStatus.Pending:
                        await _hubContext.Clients.All.Added(order);
                        break;

                    case OrderStatus.Canceled:
                        if (order.Air.Status != OrderStatus.Failed)
                        {
                            order.Air.UpdatedAt = DateTime.Now;
                            order.Air.Status = OrderStatus.Canceled;
                            
                            await _messageProducer.Publish(_airCancelTopic, orderable.OrderId);
                        }

                        if (order.Hotel.Status != OrderStatus.Failed)
                        {
                            order.Hotel.UpdatedAt = DateTime.Now;
                            order.Hotel.Status = OrderStatus.Canceled;

                            await _messageProducer.Publish(_hotelCancelTopic, orderable.OrderId);
                        }

                        await _hubContext.Clients.All.Canceled(order);
                        break;

                    case OrderStatus.Confirmed:
                        order.Air.UpdatedAt = DateTime.Now;
                        order.Air.Status = OrderStatus.Confirmed;

                        order.Hotel.UpdatedAt = DateTime.Now;
                        order.Hotel.Status = OrderStatus.Confirmed;

                        await _messageProducer.Publish(_hotelConfirmTopic, order.OrderId);
                        await _messageProducer.Publish(_airConfirmTopic, order.OrderId);

                        await _hubContext.Clients.All.Confirmed(order);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
