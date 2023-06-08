using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using MeterReadingsBot.Interfaces;
using MeterReadingsBot.Settings;

namespace MeterReadingsBot.Services;

/// <summary>
///     Представляет сервис для работы с email.
/// </summary>
public class EmailService : IEmailService
{
    #region Data
    #region Fields
    private readonly EmailSettings _emailSettings;
    #endregion
    #endregion

    #region .ctor
    /// <summary>
    ///     Инициализирует новый экземпляр типа <see cref="EmailService" />
    /// </summary>
    /// <param name="settings">Настройки email.</param>
    /// <exception cref="ArgumentNullException">Если настройки не заданы.</exception>
    public EmailService(EmailSettings settings)
    {
        _emailSettings = settings ?? throw new ArgumentNullException(nameof(settings));
    }
    #endregion

    #region IEmailService members
    /// <inheritdoc />
    public async Task SendMessageAsync(string subject, string body, string recipientMail)
    {
        var fromMail = new MailAddress(_emailSettings.BotEmail, _emailSettings.DisplayName);
        var toMail = new MailAddress(recipientMail);
        var mailMessage = new MailMessage
        {
            From = fromMail,
            To = { toMail },
            Subject = subject,
            Body = body
        };
        var smtpClient = new SmtpClient(_emailSettings.Host, _emailSettings.Port);
        smtpClient.UseDefaultCredentials = _emailSettings.UseDefaultCredentials;
        smtpClient.Credentials = new NetworkCredential(_emailSettings.BotEmail, _emailSettings.Password);
        smtpClient.EnableSsl = _emailSettings.EnableSsl;
        await smtpClient.SendMailAsync(mailMessage);
    }
    #endregion
}
