using System.Threading;
using System.Threading.Tasks;
using MeterReadingsBot.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace MeterReadingsBot.Controllers;

/// <summary>
/// Представляет контроллер телеграм бота.
/// </summary>
public class TelegramBotController : ControllerBase
{
    #region Public
    /// <summary>
    /// Отправляет сообщение в телеграм бот.
    /// </summary>
    /// <param name="handleUpdateService">Сервис обработки сообщений.</param>
    /// <param name="update">Сообщение.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Результат операции.</returns>
    [HttpPost]
    public async Task<IActionResult> PostMessage([FromServices] HandleUpdateService handleUpdateService,
        [FromBody] Update update,
        CancellationToken cancellationToken)
    {
        await handleUpdateService.EchoAsync(update, cancellationToken);
        return Ok();
    }
    #endregion
}
