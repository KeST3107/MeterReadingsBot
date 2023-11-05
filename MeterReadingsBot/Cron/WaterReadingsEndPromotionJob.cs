using System;
using System.Threading;
using System.Threading.Tasks;
using MeterReadingsBot.Interfaces;
using MeterReadingsBot.Settings;
using Microsoft.Extensions.Logging;
using Quartz;

namespace MeterReadingsBot.Cron;

/// <summary>
/// Представляет джобу массовой рассылки завершения приема показаний.
/// </summary>
public class WaterReadingsEndPromotionJob : JobBase
{
    private readonly IPromotionService _promotionService;
    private readonly PromotionMessageSettings _promotionMessageSettings;
    /// <summary>
    /// Инициализирует новый экземпляр типа <see cref="WaterReadingsEndPromotionJob" />.
    /// </summary>
    /// <param name="loggerFactory">Фабрика логгеров.</param>
    /// <param name="promotionService">Сервис массовой рассылки.</param>
    /// <param name="promotionMessageSettings">Настройки сообщений рассылки.</param>
    public WaterReadingsEndPromotionJob(ILoggerFactory loggerFactory, IPromotionService promotionService, PromotionMessageSettings promotionMessageSettings) : base(loggerFactory)
    {
        _promotionService = promotionService ?? throw new ArgumentNullException(nameof(promotionService));
        _promotionMessageSettings = promotionMessageSettings ?? throw new ArgumentNullException(nameof(promotionMessageSettings));
    }

    /// <inheritdoc />
    protected override Task InnerExecute(IJobExecutionContext context)
    {
        _promotionService.StartPromotion(_promotionMessageSettings.EndWaterReadings, CancellationToken.None, nameof(WaterReadingsEndPromotionJob));
        return Task.CompletedTask;
    }
}
