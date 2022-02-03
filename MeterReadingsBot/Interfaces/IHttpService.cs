namespace MeterReadingsBot.Interfaces
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IHttpService
    {
        public Task<HttpResponseMessage> PostAsync(Uri uri, StringContent content);
    }
}
