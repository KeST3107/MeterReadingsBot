using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace MeterReadingsBot.Services.ClientStateServices;

/// <summary>
/// Представляет сервис суперадминов.
/// </summary>
public interface IAdminUserClientService
{
    /// <summary>
    /// Возвращает стартовое сообщение для суперадмина.
    /// </summary>
    /// <param name="message">Сообщение клиента.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Задача сообщения.</returns>
    Task<Message> GetStartAdminUserTaskMessageAsync(Message message, CancellationToken cancellationToken);

    /// <summary>
    /// Возвращает сообщение сценария взаимодействия для суперадмина.
    /// </summary>
    /// <param name="message">Сообщение клиента.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Задача сообщения.</returns>
    Task<Message> GetAdminUserTaskMessage(Message message, CancellationToken cancellationToken);
}
