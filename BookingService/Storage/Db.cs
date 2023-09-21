using Model;
using System.Collections.Concurrent;

namespace BookingService.Storage
{
    public static class Db
    {
        public readonly static ConcurrentDictionary<string, Order> Orders = new ConcurrentDictionary<string, Order>();
    }
}
