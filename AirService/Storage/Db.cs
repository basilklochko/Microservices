using Model;
using System.Collections.Concurrent;

namespace AirService.Storage
{
    public static class Db
    {
        public readonly static string[] Airs = new string[] { "United", "American", "Spirit", "JetBlue", "Alaska" };

        public readonly static ConcurrentDictionary<string, Orderable> Orders = new ConcurrentDictionary<string, Orderable>();
    }
}
