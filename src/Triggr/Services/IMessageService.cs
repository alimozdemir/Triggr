namespace Triggr.Services
{
    public interface IMessageService
    {
        ActuatorType MessageType { get; }
        void Send(Data.Repository repository, Actuator act, string message);
    }
}