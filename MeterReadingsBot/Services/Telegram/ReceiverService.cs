using MeterReadingsBot.Abstract;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace MeterReadingsBot.Services.Telegram;

public class ReceiverService : ReceiverServiceBase<UpdateHandler>
{
    public ReceiverService(
        ITelegramBotClient botClient,
        UpdateHandler updateHandler,
        ILogger<ReceiverServiceBase<UpdateHandler>> logger)
        : base(botClient, updateHandler, logger)
    {
    }
}
