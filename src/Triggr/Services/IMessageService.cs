namespace Triggr.Services
{
    public interface IMessageService
    {
        ActuatorType MessageType { get; }
        void Send(Actuator act, string message);
    }
}