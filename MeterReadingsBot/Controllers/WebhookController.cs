using System.Threading.Tasks;
using MeterReadingsBot.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace MeterReadingsBot.Controllers
{
    public class WebhookController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromServices] HandleUpdateService handleUpdateService,
            [FromBody] Update update)
        {
            await handleUpdateService.EchoAsync(update);
            return Ok();
        }
    }
}
