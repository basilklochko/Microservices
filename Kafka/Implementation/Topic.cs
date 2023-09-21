using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Kafka.Interface;
using Microsoft.Extensions.Configuration;

namespace Kafka.Implementation
{
    public class Topic : ITopic, IDisposable
    {
        private readonly IAdminClient _adminClient;
        public Topic(IConfiguration configuration)
        {
            var hosts = configuration.GetSection("Kafka").GetSection("Host").Value;

            _adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = hosts }).Build();
        }

        public async Task Create(string[] names)
        {
            foreach (var name in names)
            {
                try
                {
                    await _adminClient.CreateTopicsAsync(new TopicSpecification[]
                    {
                        new TopicSpecification { Name = name, ReplicationFactor = 1, NumPartitions = 1 }
                    });
                }
                catch (Exception)
                {
                    
                }
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
