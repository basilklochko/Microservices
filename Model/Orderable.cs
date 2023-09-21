namespace Model
{
    public class Orderable
    {
        public Orderable()
        {
            UpdatedAt = DateTime.Now;
            Status = OrderStatus.Pending;
        }

        public DateTime UpdatedAt { get; set; }
        public string? Type { get; set; }
        public string? OrderId{ get; set; }
        public string? Name { get; set; }
        public OrderStatus Status { get; set; }
    }
}
