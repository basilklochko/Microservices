namespace Common.Interface
{
    public interface IHandler
    {
        Task Reserve(string data);
        Task Cancel(string data);
        Task Confirm(string data);
        Task Fail(string data);
    }
}
