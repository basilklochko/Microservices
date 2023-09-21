using Model;

namespace Common.Interface
{
    public interface IBookingHub
    {
        Task Added(Order order);
        Task Canceled(Order order);
        Task Confirmed(Order order);
        Task Added(string orderId);
    }
}
