using MeterReadingsBot.Cron;
using MeterReadingsBot.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace MeterReadingsBot.Extensions;

/// <summary>
///     Определяет класс расширения конфигуратора кварц.
/// </summary>
public static class ServiceCollectionQuartzConfiguratorExtensions
{
    #region Data
    #region Static
    private static readonly string _jobGroup = "WaterReadings";
    #endregion
    #endregion

    #region Public
    /// <summary>
    ///     Регистрирует кварц.
    /// </summary>
    /// <param name="serviceCollection">Экземпляр <see cref="IServiceCollection" />.</param>
    public static void AddQuartzService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<LoggerFactory>();
        var cronJobSettings = serviceCollection
            .BuildServiceProvider()
            .GetRequiredService<CronJobSettings>();
        serviceCollection.AddQuartz(q =>
        {
            q.SchedulerId = "MeterReadingsBot";
            q.InterruptJobsOnShutdown = true;
            q.InterruptJobsOnShutdownWithWait = true;
            q.MaxBatchSize = 5;
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.UseSimpleTypeLoader();
            q.UseInMemoryStore();
            q.UseDefaultThreadPool(10);
            q.RegisterJobs(cronJobSettings);
        });
        serviceCollection.AddQuartzHostedService(
            q => { q.WaitForJobsToComplete = true; }
        );
    }

    /// <summary>
    ///     Регистрирует джобы.
    /// </summary>
    /// <param name="quartzConfigurator">Экземпляр <see cref="IServiceCollectionQuartzConfigurator" />.</param>
    /// <param name="settings">Настройки триггеров.</param>
    public static void RegisterJobs(this IServiceCollectionQuartzConfigurator quartzConfigurator, CronJobSettings settings)
    {
        var waterReadingsStartPromotionJob = new JobKey(nameof(WaterReadingsStartPromotionJob), _jobGroup);
        quartzConfigurator.AddJob<WaterReadingsStartPromotionJob>(waterReadingsStartPromotionJob,
            j => j
                .WithDescription(nameof(WaterReadingsStartPromotionJob))
        );

        quartzConfigurator.AddTrigger(t => t
            .WithIdentity(nameof(WaterReadingsStartPromotionJob) + "Trigger")
            .ForJob(waterReadingsStartPromotionJob)
            .StartNow()
            .WithCronSchedule(settings.WaterReadingsStartPromotionJobTrigger)
            .WithDescription("Каждое 15 число месяца, в 12:00")
        );

        var waterReadingsEndPromotionJob = new JobKey(nameof(WaterReadingsEndPromotionJob), _jobGroup);
        quartzConfigurator.AddJob<WaterReadingsEndPromotionJob>(waterReadingsEndPromotionJob,
            j => j
                .WithDescription(nameof(WaterReadingsEndPromotionJob))
        );

        quartzConfigurator.AddTrigger(t => t
            .WithIdentity(nameof(WaterReadingsEndPromotionJob) + "Trigger")
            .ForJob(waterReadingsEndPromotionJob)
            .StartNow()
            .WithCronSchedule(settings.WaterReadingsEndPromotionJobTrigger)
            .WithDescription("Каждое 24 число месяца, в 12:00")
        );
    }
    #endregion
}
