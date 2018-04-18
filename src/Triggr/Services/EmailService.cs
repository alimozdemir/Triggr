namespace Triggr.Services
{
    public class EmailService : IMessageService
    {
        public ActuatorType MessageType => ActuatorType.Email;

        public void Send(string to, string message)
        {
            
        }
    }
}