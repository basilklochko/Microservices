using Model;
using System.Collections.Concurrent;

namespace HotelService.Storage
{
    public static class Db
    {
        public readonly static string[] Hotels = new string[] { "Hilton", "Mariott", "Super 8", "Red Roof", "Riu" };

        public readonly static ConcurrentDictionary<string, Orderable> Orders = new ConcurrentDictionary<string, Orderable>();
    }
}
