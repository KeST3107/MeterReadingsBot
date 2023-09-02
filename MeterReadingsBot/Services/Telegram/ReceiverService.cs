using MeterReadingsBot.Abstract;
using MeterReadingsBot.Services.ClientStateServices;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace MeterReadingsBot.Services.Telegram;

/// <summary>
/// Определяет receiver сервис.
/// </summary>
public class ReceiverService : ReceiverServiceBase<UpdateHandlerService>
{
    /// <summary>
    /// Инициализирует новый экземпляр типа <see cref="ReceiverService" />
    /// </summary>
    /// <param name="botClient">Экземпляр телеграм бота.</param>
    /// <param name="updateHandlerService">Обработчик входящих сообщений.</param>
    /// <param name="logger">Логгер.</param>
    public ReceiverService(
        ITelegramBotClient botClient,
        UpdateHandlerService updateHandlerService,
        ILogger<ReceiverServiceBase<UpdateHandlerService>> logger)
        : base(botClient, updateHandlerService, logger)
    {
    }
}
