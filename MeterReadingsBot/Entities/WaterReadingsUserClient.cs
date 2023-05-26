using System.Collections.Generic;
using MeterReadingsBot.Enums;
using MeterReadingsBot.Models;

namespace MeterReadingsBot.Entities;

/// <summary>
/// Определяет клиента передачи показаний.
/// </summary>
public class WaterReadingsUserClient : UserClientBase
{
    #region .ctor
    /// <summary>
    /// Инициализирует новый экземпляр типа <see cref="WaterReadingsUserClient" />
    /// </summary>
    /// <param name="chatId">Идентификатор чата.</param>
    public WaterReadingsUserClient(long chatId) : base(chatId)
    {
        PersonalNumbers = new List<string>();
        WaterReadingsState = WaterReadingsState.Start;
        TempClient = new Client();
    }
    #endregion

    #region Properties
    /// <summary>
    /// Возвращает коллекцию персональных номеров.
    /// </summary>
    public List<string> PersonalNumbers { get; }

    /// <summary>
    /// Возвращает временного клиента.
    /// </summary>
    public Client TempClient { get; }

    /// <summary>
    /// Возвращает состояние.
    /// </summary>
    public WaterReadingsState WaterReadingsState { get; set; }
    #endregion

    #region Public
    /// <summary>
    /// Добавляет персональный номер в коллекцию.
    /// </summary>
    /// <param name="personalNumber">Персональный номер.</param>
    public void AddPersonnelNumber(string personalNumber)
    {
        if (!PersonalNumbers.Contains(personalNumber))
        {
            PersonalNumbers.Add(personalNumber);
            UpdateLastMessage();
        }
    }

    /// <summary>
    /// Обновляет клиента.
    /// </summary>
    /// <param name="client">Модель клиента.</param>
    public void UpdateClient(ClientDto client)
    {
        TempClient.Address = client.Address;
        TempClient.FullName = client.FullName;
        TempClient.PersonalNumber = client.PersonnelNumber;
        UpdateLastMessage();
    }
    #endregion
}
