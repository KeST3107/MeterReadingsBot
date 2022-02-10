using System.Threading.Tasks;

namespace MeterReadingsBot.Interfaces
{
    public interface IEmailService
    {
        public Task SendMessageAsync(string address, int coldWater, int hotWater);
    }
}
