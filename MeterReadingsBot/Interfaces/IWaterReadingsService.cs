namespace MeterReadingsBot.Interfaces
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using MeterReadingsBot.Models;

    public interface IWaterReadingsService
    {
        public Task<HttpResponseMessage> SendReadingsAsync(int personalNumber, int hot);

        public Task<ClientInfoDto> GetClientInfo(int personalNumber);

        public Task<HttpResponseMessage> GetInformationAsync(int personalNumber);
    }
}
