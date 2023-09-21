using Confluent.Kafka;
using Kafka.Interface;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Kafka.Implementation
{
    public class Subscriber : ISubscriber, IDisposable
    {
        private readonly IConsumer<string, string> _consumer;

        private bool _cancelled;

        public event EventHandler<KafkaEventArgs>? MessageReceived = null;

        public Subscriber(IConfiguration configuration)
        {
            var hosts = configuration.GetSection("Kafka").GetSection("Host").Value;
            var group = configuration.GetSection("Kafka").GetSection("Group").Value;

            var config = new ConsumerConfig
            {
                BootstrapServers = hosts,
                GroupId = group,
                EnableAutoCommit = false,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<string, string>(config).Build();
        }

        public async Task SubscribeAndConsume(IEnumerable<string> topics, CancellationToken cancellationToken)
        {
            _consumer.Subscribe(topics);

            await Task.Run(() =>
            {
                while (!_cancelled)
                {
                    var consumeResult = _consumer.Consume(cancellationToken);

                    try
                    {
                        if (consumeResult is not null)
                        {
                            OnMessageReceived(new KafkaEventArgs()
                            {
                                Topic = consumeResult.Topic,
                                Key = consumeResult.Message.Key,
                                Data = consumeResult.Message.Value
                            });
                        }

                        _consumer.Commit(consumeResult);
                    }
                    catch (KafkaException e)
                    {
                        Debug.WriteLine($"Commit error: {e.Error.Reason}");
                    }
                }

                _consumer.Close();
            }, cancellationToken);
        }

        public void Unsubscribe()
        {
            _cancelled = true;
        }

        public void Dispose()
        {
            _consumer.Dispose();
            GC.SuppressFinalize(this);
        }

        private void OnMessageReceived(KafkaEventArgs e)
        {
            if (MessageReceived is not null)
            {
                EventHandler<KafkaEventArgs> handler = MessageReceived;

                if (handler is not null)
                {
                    handler(this, e);
                }
            }
        }
    }
}
