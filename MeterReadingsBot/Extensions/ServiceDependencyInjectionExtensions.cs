using System.Diagnostics;
using MeterReadingsBot.Interfaces;
using MeterReadingsBot.Repositories;
using MeterReadingsBot.Services;
using MeterReadingsBot.Services.ClientStateServices;
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
        services.AddScoped<IUserClientRepository, UserClientRepository>();
        services.AddScoped<IWaterReadingsClientRepository, UserClientRepository>();
        services.AddScoped<IStartUserClientRepository, UserClientRepository>();
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
        services.AddSingleton(GetWaterReadingsServiceSettings(configuration));
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
