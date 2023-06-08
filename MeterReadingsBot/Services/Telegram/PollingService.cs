using System;
using MeterReadingsBot.Abstract;
using Microsoft.Extensions.Logging;

namespace MeterReadingsBot.Services.Telegram;

public class PollingService : PollingServiceBase<ReceiverService>
{

    public PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger)
        : base(serviceProvider, logger)
    {
    }

}
