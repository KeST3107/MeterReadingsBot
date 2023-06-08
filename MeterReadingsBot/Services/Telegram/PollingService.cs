using System;
using MeterReadingsBot.Abstract;
using Microsoft.Extensions.Logging;

namespace MeterReadingsBot.Services.Telegram;

/// <summary>
/// Определяет polling сервис.
/// </summary>
public class PollingService : PollingServiceBase<ReceiverService>
{

    /// <summary>
    /// Инициализирует новый экземпляр типа <see cref="PollingService" />.
    /// </summary>
    /// <param name="serviceProvider">Провайдер сервисов.</param>
    /// <param name="logger">Логгер.</param>
    public PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger)
        : base(serviceProvider, logger)
    {
    }

}
