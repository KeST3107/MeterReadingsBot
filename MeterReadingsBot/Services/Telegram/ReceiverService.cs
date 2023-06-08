using MeterReadingsBot.Abstract;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace MeterReadingsBot.Services.Telegram;

/// <summary>
/// Определяет receiver сервис.
/// </summary>
public class ReceiverService : ReceiverServiceBase<UpdateHandler>
{
    /// <summary>
    /// Инициализирует новый экземпляр типа <see cref="ReceiverService" />
    /// </summary>
    /// <param name="botClient">Экземпляр телеграм бота.</param>
    /// <param name="updateHandler">Обработчик входящих сообщений.</param>
    /// <param name="logger">Логгер.</param>
    public ReceiverService(
        ITelegramBotClient botClient,
        UpdateHandler updateHandler,
        ILogger<ReceiverServiceBase<UpdateHandler>> logger)
        : base(botClient, updateHandler, logger)
    {
    }
}
