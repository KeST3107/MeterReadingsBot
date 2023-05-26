using System;
using System.Net.Http;
using System.Threading.Tasks;
using MeterReadingsBot.Interfaces;

namespace MeterReadingsBot.Services;

/// <summary>
///     Представляет сервис клиента http.
/// </summary>
public class HttpClientService : IHttpClientService

{
    #region Data
    #region Fields
    private readonly IHttpClientFactory _clientFactory;
    #endregion
    #endregion

    #region .ctor
    /// <summary>
    ///     Инициализирует новый экземпляр типа <see cref="HttpClientService" />
    /// </summary>
    /// <param name="clientFactory">Фабрика клиентов.</param>
    /// <exception cref="ArgumentNullException">Если фабрика клиентов не определена.</exception>
    public HttpClientService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
    }
    #endregion

    #region IHttpClientService members
    /// <inheritdoc />
    public async Task<HttpResponseMessage> PostAsync(Uri uri, StringContent content)
    {
        var client = _clientFactory.CreateClient();
        var response = await client.PostAsync(uri, content);
        return response;
    }
    #endregion
}
