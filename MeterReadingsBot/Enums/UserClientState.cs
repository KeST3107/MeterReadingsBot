namespace MeterReadingsBot.Enums;

/// <summary>
/// Представляет диалоговые состояния клиента.
/// </summary>
public enum UserClientState
{
    /// <summary>
    /// Стартовое состояние.
    /// </summary>
    Start = 0,

    /// <summary>
    /// Состояние передачи показаний.
    /// </summary>
    WaterReadings = 1
}
