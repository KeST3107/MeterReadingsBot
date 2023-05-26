using System;
using MeterReadingsBot.Controllers;
using MeterReadingsBot.Extensions;
using MeterReadingsBot.Services;
using MeterReadingsBot.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace MeterReadingsBot;

/// <summary>
/// Описывает инициализацию сервиса.
/// </summary>
public class Startup
{
    #region .ctor
    /// <summary>
    /// Инициализирует новый экземпляр типа <see cref="Startup" />
    /// </summary>
    /// <param name="configuration">Конфигурация.</param>
    /// <exception cref="ArgumentNullException">Если <see cref="IConfiguration"/> не задан.</exception>
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
    #endregion

    #region Properties
    /// <summary>
    /// Возвращает конфигурацию.
    /// </summary>
    public IConfiguration Configuration { get;}
    #endregion

    #region Public
    /// <summary>
    /// Конфигурирует окружение сервиса.
    /// </summary>
    /// <param name="app">Билдер приложения.</param>
    /// <param name="env">Веб окружение.</param>
    /// <param name="settings">Настройки телеграм бота.</param>
    /// <exception cref="ArgumentNullException">Если один из аргументов не задан.</exception>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TelegramBotSettings settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseRouting();
        app.UseCors();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute("tgwebhook",
                $"bot/{settings.BotToken}",
                new { controller = "Webhook", action = "Post" });
            endpoints.MapControllers();
        });
    }

    /// <summary>
    /// Конфигурирует ServiceCollection контейнер.
    /// </summary>
    /// <param name="services">ServiceCollection контейнер.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSettings(Configuration);
        services.AddServices();
        services.AddDatabase(Configuration);
        var telegramBotSettings = services
            .BuildServiceProvider()
            .GetRequiredService<TelegramBotSettings>();
        services.AddHostedService<ConfigureTelegramBotWebhook>();
        services.AddHttpClient("tgwebhook")
            .AddTypedClient<ITelegramBotClient>(httpClient
                => new TelegramBotClient(telegramBotSettings.BotToken, httpClient));
        services.AddTransient<HandleUpdateService>();
        services.AddControllers().AddNewtonsoftJson();
    }
    #endregion
}
