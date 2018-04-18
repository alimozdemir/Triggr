namespace Triggr.Services
{
    public interface IMessageFactory
    {
         IMessageService GetMessageService(ActuatorType type);
    }
}