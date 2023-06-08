using System.Threading.Tasks;

namespace MeterReadingsBot.Interfaces;

/// <summary>
///     Определяет сервис для работы с email.
/// </summary>
public interface IEmailService
{
    #region Overridable
    /// <summary>
    ///     Отправляет сообщение.
    /// </summary>
    /// <param name="subject">Тема сообщения.</param>
    /// <param name="body">Тело сообщения.</param>
    /// <param name="recipientMail">Email получателя.</param>
    /// <returns>Задача.</returns>
    public Task SendMessageAsync(string subject, string body, string recipientMail);
    #endregion
}
