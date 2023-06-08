using System.ComponentModel.DataAnnotations;

namespace MeterReadingsBot.Settings;

/// <summary>
///     Представляет настройки для Email.
/// </summary>
public class EmailSettings
{
    #region Properties
    /// <summary>
    ///     Возвращает почтовый адрес бота.
    /// </summary>
    [Required]
    public string BotEmail { get; init; } = null!;

    /// <summary>
    ///     Возвращает отображаемое имя.
    /// </summary>
    [Required]
    public string DisplayName { get; init; } = null!;

    /// <summary>
    ///     Возвращает флаг разрешения ssl.
    /// </summary>
    [Required]
    public bool EnableSsl { get; init; }

    /// <summary>
    ///     Возвращает адрес хоста.
    /// </summary>
    [Required]
    public string Host { get; init; } = null!;

    /// <summary>
    ///     Возвращает пароль доступа к email.
    /// </summary>
    [Required]
    public string Password { get; init; } = null!;

    /// <summary>
    ///     Возвращает порт.
    /// </summary>
    [Required]
    public int Port { get; init; }

    /// <summary>
    ///     Возвращает флаг использования учетных данных по умолчанию.
    /// </summary>
    [Required]
    public bool UseDefaultCredentials { get; init; }
    #endregion
}
