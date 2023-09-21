namespace Model
{
    public class Order
    {
        public Order()
        {
            UpdatedAt = DateTime.Now;
            Status = OrderStatus.Pending;
        }

        public Orderable? Air { get; set; }
        
        public Orderable? Hotel { get; set; }

        public DateTime UpdatedAt { get; set; }

        public OrderStatus Status
        {
            get
            {
                if (Air is not null && Hotel is not null)
                {
                    if (string.IsNullOrEmpty(Air.Name) || string.IsNullOrEmpty(Hotel.Name))
                    {
                        return OrderStatus.Canceled;
                    }
                }

                if (!string.IsNullOrEmpty(Air?.Name) && !string.IsNullOrEmpty(Hotel?.Name))
                {
                    return OrderStatus.Confirmed;
                }

                return OrderStatus.Pending;
            }
            init
            {

            }
        }

        public string? OrderId
        {
            get
            {
                var result = string.Empty;

                if (Air is not null || Hotel is not null)
                {
                    result = Air is null ? Hotel?.OrderId : Air.OrderId;
                }

                return result;
            }
            set { }
        }
    }
}