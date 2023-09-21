namespace Kafka
{
    public class KafkaEventArgs : EventArgs
    {
        public string? Topic { get; set; }
        public string? Key { get; set; }
        public string? Data { get; set; }
    }
}
