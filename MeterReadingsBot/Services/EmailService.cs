using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using MeterReadingsBot.Interfaces;
using MeterReadingsBot.Models.Configuration;
using Microsoft.Extensions.Configuration;

namespace MeterReadingsBot.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfiguration;

        public EmailService(IConfiguration configuration)
        {
            _emailConfiguration = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
        }

        public async Task SendMessageAsync(string address, int coldWater, int hotWater)
        {
            var fromMail = new MailAddress(_emailConfiguration.BotEmail, "Передача показаний бот");
            var toMail = new MailAddress(_emailConfiguration.ToEmail);
            var mailMessage = new MailMessage(fromMail, toMail);
            mailMessage.Subject = address;
            mailMessage.Body = "Показания\n" +
                               $"Холодная вода: {coldWater}\n" +
                               $"Горячая вода: {hotWater}\n" +
                               "Данное сообщение сформировано автоматически и не требует ответа!";
            var smtpClient = new SmtpClient(_emailConfiguration.Host, _emailConfiguration.Port);
            smtpClient.UseDefaultCredentials = _emailConfiguration.UseDefaultCredentials;
            smtpClient.Credentials = new NetworkCredential(_emailConfiguration.BotEmail, _emailConfiguration.Password);
            smtpClient.EnableSsl = _emailConfiguration.EnableSsl;
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
