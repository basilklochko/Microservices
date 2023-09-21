namespace Kafka.Interface
{
    public interface ISubscriber
    {
        event EventHandler<KafkaEventArgs> MessageReceived;

        Task SubscribeAndConsume(IEnumerable<string> topics, CancellationToken cancellationToken);

        void Unsubscribe();
    }
}
