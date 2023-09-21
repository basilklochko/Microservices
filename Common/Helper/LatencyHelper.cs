namespace Common.Helper
{
    public static class LatencyHelper
    {
        public static async Task Delay()
        {
            await Task.Delay(new Random().Next(1000, 3000));
        }
    }
}
