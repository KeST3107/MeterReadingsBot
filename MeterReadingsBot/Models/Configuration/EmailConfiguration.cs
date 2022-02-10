namespace MeterReadingsBot.Models.Configuration
{
    public class EmailConfiguration
    {
        public string BotEmail { get; init; }
        public string Password { get; init; }
        public string Host { get; init; }
        public int Port { get; init; }
        public bool UseDefaultCredentials { get; init; }
        public bool EnableSsl { get; init; }
        public string ToEmail { get; init; }
    }
}
