using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace MeterReadingsBot.Interfaces
{
    public interface IMessageHandler
    {
        public Task<string> Handle(Message message);
    }
}
