using MeterReadingsBot.Enums;

namespace MeterReadingsBot.Entities;

/// <summary>
/// Определяет суперадмина.
/// </summary>
public class AdminUserClient : UserClientBase
{
    /// <summary>
    /// Инициализирует новый экземпляр типа <see cref="StartUserClient" />.
    /// </summary>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <param name="userName">Логин пользователя или его чат идентификатор.</param>
    public AdminUserClient(long chatId, string userName) : base(chatId)
    {
        AdminUserState = AdminUserState.Start;
        UserName = userName;
    }

    private AdminUserClient()
    {

    }

    /// <summary>
    /// Возвращает состояние.
    /// </summary>
    public AdminUserState AdminUserState { get;  set; }

    /// <summary>
    /// Возвращает UserName пользователя телеграм или его id.
    /// </summary>
    public string UserName { get; set; }
}
