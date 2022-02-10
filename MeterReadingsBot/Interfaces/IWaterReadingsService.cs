using System.Net.Http;
using System.Threading.Tasks;
using MeterReadingsBot.Models;

namespace MeterReadingsBot.Interfaces
{
    public interface IWaterReadingsService
    {
        public Task<HttpResponseMessage> SendReadingsAsync(int personalNumber, int hot);

        public Task<ClientInfoDto> GetClientInfoAsync(int personalNumber);
    }
}
