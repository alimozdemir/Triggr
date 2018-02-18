namespace Triggr.Services
{
    public interface IMessageService
    {
        void Send(string to, string message);
    }
}