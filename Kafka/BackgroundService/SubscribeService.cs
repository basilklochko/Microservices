using Common.Interface;
using Kafka.Interface;

namespace Kafka.BackgroundService
{
    public class SubscribeService : BackgroundService
    {
        private readonly List<string> _topics = new();
        private readonly ISubscriber _subscriber;
        private readonly IHandler _handler;

        public SubscribeService(ISubscriber subscriber, string topic, string cancelTopic, string confirmTopic, string failTopic, IHandler handler)
        {
            _topics.Add(topic);

            if (!string.IsNullOrEmpty(cancelTopic))
            {
                _topics.Add(cancelTopic);
            }

            if (!string.IsNullOrEmpty(confirmTopic))
            {
                _topics.Add(confirmTopic);
            }

            if (!string.IsNullOrEmpty(failTopic))
            {
                _topics.Add(failTopic);
            }

            _handler = handler;

            _subscriber = subscriber;
            _subscriber.MessageReceived += OnMessageReceived;
        }

        private async void OnMessageReceived(object? sender, KafkaEventArgs e)
        {
            if (e.Topic.Contains("-cancel"))
            {
                _handler.Cancel(e.Data);
                return;
            }

            if (e.Topic.Contains("-fail"))
            {
                _handler.Fail(e.Data);
                return;
            }

            if (e.Topic.Contains("-confirm"))
            {
                _handler.Confirm(e.Data);
                return;
            }

            await _handler.Reserve(e.Data);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _subscriber.SubscribeAndConsume(_topics, stoppingToken);
        }
    }
}
