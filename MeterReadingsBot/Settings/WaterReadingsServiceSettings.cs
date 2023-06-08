using System.ComponentModel.DataAnnotations;

namespace MeterReadingsBot.Settings;

/// <summary>
///     Представляет настройки сервиса передачи показаний.
/// </summary>
public class WaterReadingsServiceSettings
{
    #region Properties
    /// <summary>
    ///     Возвращает дату начала приема показаний.
    /// </summary>
    [Required]
    public int DateFrom { get; init; }

    /// <summary>
    ///     Возвращает дату конца приема показаний.
    /// </summary>
    [Required]
    public int DateTo { get; init; }

    /// <summary>
    ///     Возвращает uri запроса получения данных о клиенте.
    /// </summary>
    [Required]
    public string GetClientUri { get; init; } = null!;

    /// <summary>
    ///     Возвращает uri запроса отправки показаний.
    /// </summary>
    [Required]
    public string SendReadingsUri { get; init; } = null!;

    /// <summary>
    ///     Возвращает почтовый адрес гороводоканала для клиентов Вычегодского.
    /// </summary>
    [Required]
    public string DTVSEmail { get; init; } = null!;

    /// <summary>
    ///     Возвращает почтовый адрес гороводоканала для клиентов Котласа.
    /// </summary>
    [Required]
    public string KTLSEmail { get; init; } = null!;
    #endregion
}
