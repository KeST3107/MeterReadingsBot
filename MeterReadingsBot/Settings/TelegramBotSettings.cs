using System.ComponentModel.DataAnnotations;

namespace MeterReadingsBot.Settings;

/// <summary>
///     Представляет настройки телеграм бота.
/// </summary>
public class TelegramBotSettings
{
    #region Properties
    /// <summary>
    ///     Возвращает токен бота.
    /// </summary>
    [Required]
    public string BotToken { get; init; } = null!;
    #endregion
}
