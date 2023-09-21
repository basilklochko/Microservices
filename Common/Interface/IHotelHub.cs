using Model;

namespace Common.Interface
{
    public interface IHotelHub
    {
        Task Added(Orderable order);
        Task Canceled(Orderable order);
        Task Confirmed(Orderable order);
        Task Failed(Orderable order);
    }
}
