using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

namespace MeterReadingsBot.Cron;

/// <summary>
/// Определяет базовыйй класс джобы.
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
        var taskType = GetType();
        _logger.LogInformation("Запуск таски под названием: {TaskType}.", taskType);
        try
        {
            _logger.LogInformation("Обработка таски: {NameTask} запустилась.", context.JobDetail.Key.Name);
            await InnerExecute(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Таска завершилась: {NameTask} с ошибкой.", context.JobDetail.Key.Name);
            throw new JobExecutionException(ex);
        }
        _logger.LogInformation("Обработка таски: {NameTask} завершилась.", context.JobDetail.Key.Name);
    }
}
