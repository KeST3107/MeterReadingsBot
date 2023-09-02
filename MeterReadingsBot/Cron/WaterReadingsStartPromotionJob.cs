using System;
using System.Threading;
using System.Threading.Tasks;
using MeterReadingsBot.Interfaces;
using MeterReadingsBot.Settings;
using Microsoft.Extensions.Logging;
using Quartz;

namespace MeterReadingsBot.Cron;

/// <summary>
/// Представляет джобу массовой рассылки начала передачи показаний.
/// </summary>
public class WaterReadingsStartPromotionJob : JobBase
{
    private readonly IPromotionService _promotionService;
    private readonly PromotionMessageSettings _promotionMessageSettings;
    /// <summary>
    /// Инициализирует новый экземпляр типа <see cref="WaterReadingsStartPromotionJob" />.
    /// </summary>
    /// <param name="loggerFactory">Фабрика логгеров.</param>
    /// <param name="promotionService">Сервис массовой рассылки.</param>
    /// <param name="promotionMessageSettings">Настройки сообщений.</param>
    public WaterReadingsStartPromotionJob(ILoggerFactory loggerFactory, IPromotionService promotionService, PromotionMessageSettings promotionMessageSettings) : base(loggerFactory)
    {
        _promotionService = promotionService ?? throw new ArgumentNullException(nameof(promotionService));
        _promotionMessageSettings = promotionMessageSettings ?? throw new ArgumentNullException(nameof(promotionMessageSettings));
    }

    /// <inheritdoc />
    protected override Task InnerExecute(IJobExecutionContext context)
    {
        _promotionService.StartPromotion(_promotionMessageSettings.StartWaterReadings, CancellationToken.None, nameof(WaterReadingsStartPromotionJob));
        return Task.CompletedTask;
    }
}
