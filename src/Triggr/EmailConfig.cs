namespace Triggr
{
    public class EmailConfig
    {
        public string SmtpServer { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string From { get; set; }
        public int Port { get; set; }
        public bool Ssl { get; set; }
    }
}