using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace Triggr.Services
{
    public class EmailService : IMessageService
    {
        private readonly EmailConfig _emailConfig;

        public EmailService(IOptions<EmailConfig> config)
        {
            _emailConfig = config.Value;
        }

        public ActuatorType MessageType => ActuatorType.Email;

        public void Send(Data.Repository repository, Actuator act, string message)
        {
            SmtpClient client = new SmtpClient(_emailConfig.SmtpServer, _emailConfig.Port);
            
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_emailConfig.Username, _emailConfig.Password);

            if (_emailConfig.Ssl)
                client.EnableSsl = true;

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_emailConfig.From);

            foreach (var to in act.Emails)
                mailMessage.To.Add(to);

            mailMessage.Body = message;
            mailMessage.Subject = "Triggr Notification";

            client.Send(mailMessage);
        }
    }
}