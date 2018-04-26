using System.Net;
using System.Net.Mail;

namespace Triggr.Services
{
    public class EmailService : IMessageService
    {
        public EmailService()
        {
            
        }
        
        public ActuatorType MessageType => ActuatorType.Email;

        public void Send(Probe probe, string message)
        {
            SmtpClient client = new SmtpClient("mysmtpserver");
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("username", "password");

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("whoever@me.com");
            mailMessage.To.Add("receiver@me.com");
            mailMessage.Body = "body";
            mailMessage.Subject = "subject";
            client.Send(mailMessage);
        }
    }
}