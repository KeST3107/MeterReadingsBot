using System;
using System.Threading;
using System.Threading.Tasks;
using MeterReadingsBot.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace MeterReadingsBot.Controllers;

/// <summary>
/// Представляет конфигурацию Webhook.
/// </summary>
public class ConfigureTelegramBotWebhook : IHostedService
{
    #region Data
    #region Fields
    private readonly ILogger<ConfigureTelegramBotWebhook> _logger;
    private readonly IServiceProvider _services;
    private readonly TelegramBotSettings _telegramBotSettings;
    #endregion
    #endregion

    #region .ctor
    /// <summary>
    /// Инициализирует новый экземпляр типа <see cref="ConfigureTelegramBotWebhook" />
    /// </summary>
    /// <param name="logger">Логгер.</param>
    /// <param name="serviceProvider">Провайдер.</param>
    /// <param name="telegramBotSettings">Настройки телеграм бота.</param>
    /// <exception cref="ArgumentNullException">Если один из аргументов не задан.</exception>
    public ConfigureTelegramBotWebhook(ILogger<ConfigureTelegramBotWebhook> logger,
        IServiceProvider serviceProvider,
        TelegramBotSettings telegramBotSettings)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _services = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _telegramBotSettings = telegramBotSettings ?? throw new ArgumentNullException(nameof(telegramBotSettings));
    }
    #endregion

    #region IHostedService members
    /// <summary>
    /// Устанавливает вебхук.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
        var webhookAddress = @$"{_telegramBotSettings.HostAddress}/bot/{_telegramBotSettings.BotToken}";

        _logger.LogInformation($"Setting webhook: {webhookAddress}");
        try
        {
            await botClient.SetWebhookAsync(
                webhookAddress,
                allowedUpdates: Array.Empty<UpdateType>(),
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error when setting webhook: {webhookAddress}");
            throw;
        }
    }

    /// <summary>
    /// Удаляет вебхук.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
        _logger.LogInformation("Removing webhook");
        try
        {
            await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error when removing webhook: {botClient.BotId}");
            throw;
        }
    }
    #endregion
}
