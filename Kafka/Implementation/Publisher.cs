using Confluent.Kafka;
using Kafka.Interface;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text.Json;

namespace Kafka.Implementation
{
    public class Publisher : IPublisher, IDisposable
    {
        private readonly IProducer<string, string> _producer;

        public Publisher(IConfiguration configuration)
        {
            var hosts = configuration.GetSection("Kafka").GetSection("Host").Value;

            var config = new ProducerConfig
            {
                BootstrapServers = hosts,
                ClientId = Dns.GetHostName()
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task<Guid> Publish<T>(string topic, T message)
        {
            var key = Guid.NewGuid();

            await _producer.ProduceAsync(topic, new Message<string, string> { Key = key.ToString(), Value = JsonSerializer.Serialize(message) });
            _producer.Flush(TimeSpan.FromSeconds(10));

            return key;
        }

        public void Dispose()
        {
            _producer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
