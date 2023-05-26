using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace MeterReadingsBot.Services.ClientStateServices;

/// <summary>
/// Представляет сервис клиентов передачи показаний.
/// </summary>
public interface IWaterReadingsUserClientService
{
    #region Overridable
    /// <summary>
    /// Возвращает стартовое сообщение передачи показаний для клиента.
    /// </summary>
    /// <param name="message">Сообщение клиента.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Задача сообщения.</returns>
    Task<Message> GetStartWaterReadingsTaskMessage(Message message, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает сообщение сценария передачи показаний для клиента.
    /// </summary>
    /// <param name="message">Сообщение клиента.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Задача сообщения.</returns>
    Task<Message> GetWaterReadingsTaskMessage(Message message, CancellationToken cancellationToken);
    #endregion
}
