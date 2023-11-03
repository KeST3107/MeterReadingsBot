using System.ComponentModel.DataAnnotations;

namespace MeterReadingsBot.Settings;

/// <summary>
///     Определяет сообщения рассылок.
/// </summary>
public class PromotionMessageSettings
{
    #region Properties
    /// <summary>
    ///     Возвращает сообщение завершения передачи показаний.
    /// </summary>
    [Required]
    public string EndWaterReadings { get; init; } = null!;

    /// <summary>
    ///     Возвращает сообщение начала передачи показаний.
    /// </summary>
    [Required]
    public string StartWaterReadings { get; init; } = null!;
    #endregion
}
