using Model;

namespace Common.Interface
{
    public interface IAirHub
    {
        Task Added(Orderable order);
        Task Canceled(Orderable order);
        Task Confirmed(Orderable order);
        Task Failed(Orderable order);
    }
}
