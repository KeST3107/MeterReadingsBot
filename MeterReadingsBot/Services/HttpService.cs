using System;
using System.Net.Http;
using System.Threading.Tasks;
using MeterReadingsBot.Interfaces;

namespace MeterReadingsBot.Services
{
    public class HttpService : IHttpService

    {
        private readonly IHttpClientFactory _clientFactory;

        public HttpService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public async Task<HttpResponseMessage> PostAsync(Uri uri, StringContent content)
        {
            var client = _clientFactory.CreateClient();
            var response = await client.PostAsync(uri, content);
            return response;
        }
    }
}
