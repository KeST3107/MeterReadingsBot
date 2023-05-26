namespace MeterReadingsBot.Enums;

/// <summary>
///     Представляет состояния клиента при передаче показаний.
/// </summary>
public enum WaterReadingsState
{
    /// <summary>
    ///     Стартовое состояние клиента.
    /// </summary>
    Start = 0,

    /// <summary>
    ///     Состояние передачи лицевого номера.
    /// </summary>
    PersonalNumber = 1,

    /// <summary>
    ///     Состояние подтверждения лицевого номера.
    /// </summary>
    ConfirmClientInfo = 2,

    /// <summary>
    ///     Состояние передачи холодной воды по счетчику санузла.
    /// </summary>
    ColdWaterBathroom = 3,

    /// <summary>
    ///     Состояние передачи горячей воды по счетчику санузла.
    /// </summary>
    HotWaterBathroom = 4,

    /// <summary>
    ///     Состояние передачи холодной воды по счетчику кухни.
    /// </summary>
    ColdWaterKitchen = 5,

    /// <summary>
    ///     Состояние передачи горячей воды по счетчику кухни.
    /// </summary>
    HotWaterKitchen = 6,

    /// <summary>
    ///     Состояние подтверждения показаний по счетчикам.
    /// </summary>
    ConfirmWaterReadings = 7,

    /// <summary>
    ///     Состояние продолжения передачи показаний.
    /// </summary>
    ContinueSendWaterReadings = 8
}
