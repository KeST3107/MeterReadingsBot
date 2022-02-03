namespace MeterReadingsBot.Models.Configuration
{
    public class EmailConfiguration
    {
        public string BotEmail { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public bool EnableSsl { get; set; }
        public string ToEmail { get; set; }
    }
}
