using System.Threading;
using System.Threading.Tasks;

namespace MeterReadingsBot.Abstract;

/// <summary>
/// Представляет интерфес сервиса получения сообщений.
/// </summary>
public interface IReceiverService
{
    /// <summary>
    /// Отправляет новые входящие сообщения в обработчик.
    /// </summary>
    /// <param name="stoppingToken">Токен отмены.</param>
    /// <returns>Задача.</returns>
    Task ReceiveAsync(CancellationToken stoppingToken);
}
