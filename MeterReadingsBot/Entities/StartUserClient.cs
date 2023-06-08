using MeterReadingsBot.Enums;

namespace MeterReadingsBot.Entities;

/// <summary>
/// Определяет клиента с состояними диалога.
/// </summary>
public class StartUserClient : UserClientBase
{
    #region .ctor
    /// <summary>
    /// Инициализирует новый экземпляр типа <see cref="StartUserClient" />
    /// </summary>
    /// <param name="chatId">Идентификатор чата.</param>
    public StartUserClient(long chatId) : base(chatId)
    {
        State = UserClientState.Start;
    }
    private StartUserClient()
    {

    }
    #endregion

    #region Properties
    /// <summary>
    /// Возвращает состояние клиента.
    /// </summary>
    public UserClientState State { get; private set; }
    #endregion

    #region Public
    /// <summary>
    /// Устанавливает стартовое состояние.
    /// </summary>
    public void SetStateToStartState()
    {
        State = UserClientState.Start;
    }

    /// <summary>
    /// Устанавливает состояние передачи показаний.
    /// </summary>
    public void SetStateToWaterReadingsState()
    {
        State = UserClientState.WaterReadings;
    }
    #endregion
}
