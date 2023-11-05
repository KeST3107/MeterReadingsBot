using System;
using System.Diagnostics;
using MeterReadingsBot.Interfaces;
using MeterReadingsBot.Repositories;
using MeterReadingsBot.Services;
using MeterReadingsBot.Services.ClientStateServices;
using MeterReadingsBot.Services.Telegram;
using MeterReadingsBot.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MeterReadingsBot.Extensions;

/// <summary>
///     Определяет расширения <see cref="IServiceCollection" /> для работы сервисов и конфигурации настроек.
/// </summary>
public static class ServiceDependencyInjectionExtensions
{
    #region Public
    /// <summary>
    ///     Добавляет зависимости сервисов.
    /// </summary>
    /// <param name="services">Сервисы.</param>
    public static void AddServices(this IServiceCollection services)
    {
        services.AddTransient<IHttpClientService, HttpClientService>();
        services.AddTransient<IWaterReadingsService, WaterReadingsService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IHtmlParserService, HtmlParserService>();
        services.AddTransient<IWaterReadingsUserClientService, WaterReadingsUserClientService>();
        services.AddTransient<IAdminUserClientService, AdminUserClientService>();
        services.AddTransient<IPromotionService, PromotionService>();
        services.AddScoped<IUserClientRepository, UserClientRepository>();
        services.AddScoped<IWaterReadingsClientRepository, UserClientRepository>();
        services.AddScoped<IStartUserClientRepository, UserClientRepository>();
        services.AddScoped<IAdminUserRepository, UserClientRepository>();
    }

    /// <summary>
    ///     Добавляет зависимости сервисов для телеграм бота.
    /// </summary>
    /// <param name="services">Сервисы.</param>
    public static void AddTelegramServices(this IServiceCollection services)
    {
        services.AddScoped<UpdateHandlerService>();
        services.AddScoped<ReceiverService>();
        services.AddHostedService<PollingService>();
    }
    /// <summary>
    ///     Добавляет зависимости настроек сервисов.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <param name="configuration">Конфигурация.</param>
    public static void AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration.GetSection(nameof(TelegramBotSettings)).Get<TelegramBotSettings>());
        services.AddSingleton(configuration.GetSection(nameof(EmailSettings)).Get<EmailSettings>());
        services.AddSingleton(configuration.GetSection(nameof(AdminUserSettings)).Get<AdminUserSettings>());
        services.AddSingleton(configuration.GetSection(nameof(PromotionMessageSettings)).Get<PromotionMessageSettings>());
        services.AddSingleton(GetCronJobSettings(configuration));
        services.AddSingleton(GetWaterReadingsServiceSettings(configuration));
    }

    private static CronJobSettings GetCronJobSettings(IConfiguration configuration)
    {
        return Debugger.IsAttached
            ? new CronJobSettings
            {
                WaterReadingsStartPromotionJobTrigger = "10 0/1 * * * ?", // Каждая десятая секунда минуты.
                WaterReadingsEndPromotionJobTrigger = "30 0/1 * * * ?" // Каждая тридцатая секунда минуты.
            }
            : configuration.GetSection(nameof(CronJobSettings)).Get<CronJobSettings>();
    }
    #endregion

    #region Private
    private static WaterReadingsServiceSettings GetWaterReadingsServiceSettings(IConfiguration configuration)
    {
        return Debugger.IsAttached
            ? new WaterReadingsServiceSettings
            {
                DateFrom = 0,
                DateTo = 32,
                DTVSEmail = "kest3107@gmail.com",
                KTLSEmail = "kest3107@gmail.com",
                GetClientUri = "https://www.kotlas-okits.ru/consumer/pokazaniya/input.php",
                SendReadingsUri = "https://www.kotlas-okits.ru/consumer/pokazaniya/detail.php"
            }
            : configuration.GetSection(nameof(WaterReadingsServiceSettings)).Get<WaterReadingsServiceSettings>();
    }
    #endregion
}
