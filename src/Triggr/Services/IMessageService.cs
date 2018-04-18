namespace Triggr.Services
{
    public interface IMessageService
    {
        ActuatorType MessageType { get; }
        void Send(string to, string message);
    }
}