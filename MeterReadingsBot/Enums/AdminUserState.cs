namespace MeterReadingsBot.Enums;

/// <summary>
/// Представляет диалоговые состояния суперадмина.
/// </summary>
public enum AdminUserState
{
    /// <summary>
    /// Стартовое состояние.
    /// </summary>
    Start = 0,
    /// <summary>
    /// Состояние основных комманд.
    /// </summary>
    Commands = 1,
    /// <summary>
    /// Состояние добавления админа.
    /// </summary>
    AddAdmin = 2,
    /// <summary>
    /// Состояние удаления админа.
    /// </summary>
    RemoveAdmin = 3,
    /// <summary>
    /// Состояние настройки рассылки.
    /// </summary>
    Promotion = 4,

}
