namespace MeterReadingsBot.Interfaces
{
    using System.Threading.Tasks;

    public interface IEmailService
    {
        public Task SendReadingsAsync(string address, int coldWater, int hotWater);
    }
}
