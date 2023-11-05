using System;
using System.Threading.Tasks;
using MeterReadingsBot.Properties;
using Microsoft.Extensions.Logging;
using Quartz;

namespace MeterReadingsBot.Cron;

/// <summary>
/// Определяет базовый класс джобы.
/// </summary>
public abstract class JobBase : IJob
{
    /// <summary>
    /// Фабрика логгеров.
    /// </summary>
    protected readonly ILoggerFactory LoggerFactory;
    private readonly ILogger<JobBase> _logger;

    /// <summary>
    /// Инициализирует новый экземпляр типа <see cref="JobBase" />.
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <exception cref="ArgumentNullException"></exception>
    protected JobBase(ILoggerFactory loggerFactory)
    {
        LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _logger = LoggerFactory.CreateLogger<JobBase>();
    }

    /// <summary>
    /// Внутренняя обработка джобы.
    /// </summary>
    /// <param name="context">Контекст.</param>
    /// <returns>Задача.</returns>
    protected abstract Task InnerExecute(IJobExecutionContext context);

    /// <summary>
    /// Выполняет запуск джобы.
    /// </summary>
    /// <param name="context">Контекст.</param>
    /// <exception cref="JobExecutionException">Возникает если джоба выполнилась с ошибкой.</exception>
    public async Task Execute(IJobExecutionContext context)
    {
        var jobName = GetType().Name;
        try
        {
            _logger.LogInformation(Resources.StrFmtInfoJobProcessStarted, jobName);
            await InnerExecute(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,Resources.StrFmtErrorJobEndedWithError, jobName);
            throw new JobExecutionException(ex);
        }
        _logger.LogInformation(Resources.StrFmtInfoJobEndedSuccessful, jobName);
    }
}
