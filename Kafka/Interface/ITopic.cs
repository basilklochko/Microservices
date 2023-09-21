namespace Kafka.Interface
{
    public interface ITopic
    {
        Task Create(string[] names);
    }
}
