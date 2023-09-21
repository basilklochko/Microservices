using BookingService.Storage;
using Kafka.Interface;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace BookingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IPublisher _messageProducer;
        private readonly string _hotelTopic;
        private readonly string _airTopic;

        public BookingController(IConfiguration configuration, IPublisher messageProducer)
        {
            _hotelTopic = configuration.GetSection("Kafka").GetSection("HotelTopic").Value;
            _airTopic = configuration.GetSection("Kafka").GetSection("AirTopic").Value;

            _messageProducer = messageProducer;
        }

        [HttpGet]
        public async Task<List<Order>> Get()
        {
            return await Task.Run(() =>
            {
                return Db.Orders.Select(o => o.Value).OrderByDescending(o => o.UpdatedAt).ToList();
            });
        }

        [HttpPost]
        public async Task<string> Reserve()
        {
            var id = Guid.NewGuid().ToString();

            await _messageProducer.Publish(_hotelTopic, id);
            await _messageProducer.Publish(_airTopic, id);

            return id;
        }
    }
}
