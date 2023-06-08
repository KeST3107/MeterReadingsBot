using System.Net;
using System.Threading.Tasks;
using MeterReadingsBot.Entities;
using MeterReadingsBot.Models;

namespace MeterReadingsBot.Interfaces;

/// <summary>
///     Определяет сервис передачи и получения информации о показаниях воды.
/// </summary>
public interface IWaterReadingsService
{
    #region Overridable
    /// <summary>
    ///     Получает информацию о клиенте по персональному номеру.
    /// </summary>
    /// <param name="personnelNumber">Персональный номер.</param>
    /// <returns>Данные клиента.</returns>
    Task<ClientDto> GetClientInfoAsync(int personnelNumber);

    /// <summary>
    ///     Отправляет показания горячей и холодной воды клиента в компанию СеВДту.
    /// </summary>
    /// <param name="clientInfo">Информация клиента.</param>
    /// <returns>Задача.</returns>
    Task SendWaterReadingsToGorvodokanalAsync(Client clientInfo);

    /// <summary>
    ///     Отправляет показания горячей воды клиента в компанию ОК и ТС.
    /// </summary>
    /// <param name="clientInfo">Информация клиента.</param>
    /// <returns>Ответное сообщение.</returns>
    Task<HttpStatusCode> SendWaterReadingsOKiTSAsync(Client clientInfo);
    #endregion
}
