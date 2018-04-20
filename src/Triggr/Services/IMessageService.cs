namespace Triggr.Services
{
    public interface IMessageService
    {
        ActuatorType MessageType { get; }
        void Send(Probe probe, string message);
    }
}