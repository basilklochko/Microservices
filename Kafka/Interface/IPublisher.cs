namespace Kafka.Interface
{
    public interface IPublisher
    {
        Task<Guid> Publish<T>(string topic, T message);
    }
}
