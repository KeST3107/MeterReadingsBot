using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MeterReadingsBot.Interfaces;

/// <summary>
///     Определяет сервис клиента http.
/// </summary>
public interface IHttpClientService
{
    #region Overridable
    /// <summary>
    ///     Отправляет POST запрос.
    /// </summary>
    /// <param name="uri">Идентификатор ресурса.</param>
    /// <param name="content">Контент ресурса.</param>
    /// <returns>Ответное сообщение.</returns>
    public Task<HttpResponseMessage> PostAsync(Uri uri, StringContent content);
    #endregion
}
