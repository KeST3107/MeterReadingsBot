using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MeterReadingsBot.Interfaces
{
    public interface IHttpService
    {
        public Task<HttpResponseMessage> PostAsync(Uri uri, StringContent content);
    }
}
