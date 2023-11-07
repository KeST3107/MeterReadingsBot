using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace MeterReadingsBot.Services.ClientStateServices;

/// <summary>
/// Представляет сервис работы с клиентами.
/// </summary>
public interface IUserClientService
{
    /// <summary>
    /// Возвращает стартовое сообщение.
    /// </summary>
    /// <param name="message">Сообщение клиента.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Задача сообщения.</returns>
    Task<Message> GetStartUserTaskMessageAsync(Message message, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает сообщение сценария взаимодействия.
    /// </summary>
    /// <param name="message">Сообщение клиента.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Задача сообщения.</returns>
    Task<Message> GetUserTaskMessage(Message message, CancellationToken cancellationToken);
}
