namespace MeterReadingsBot.Settings;

/// <summary>
///     Определяет настройки триггеров для джоб.
/// </summary>
public class CronJobSettings
{
    #region Properties
    /// <summary>
    ///     Возвращает триггер джобы рассылки завершения приема показаний.
    /// </summary>
    public string WaterReadingsEndPromotionJobTrigger { get; init; } = null!;

    /// <summary>
    ///     Возвращает триггер джобы рассылки начала приема показаний.
    /// </summary>
    public string WaterReadingsStartPromotionJobTrigger { get; init; } = null!;
    #endregion
}
