namespace MeterReadingsBot.Models;

/// <summary>
/// Модель данных клиента.
/// </summary>
public class ClientDto
{
    #region Properties
    /// <summary>
    /// Инициализирует новый экземпляр типа <see cref="ClientDto" />
    /// </summary>
    /// <param name="address">Адрес.</param>
    /// <param name="fullName">Полное имя.</param>
    /// <param name="personnelNumber">Персональный номер.</param>
    public ClientDto(string address, string fullName, string personnelNumber)
    {
        Address = address;
        FullName = fullName;
        PersonnelNumber = personnelNumber;
    }

    /// <summary>
    /// Возвращает адрес.
    /// </summary>
    public string Address { get; }
    /// <summary>
    /// Возвращает полное имя.
    /// </summary>
    public string FullName { get; }

    /// <summary>
    /// Возвращает персональный номер.
    /// </summary>
    public string PersonnelNumber { get;}
    #endregion
}
