using System;
using System.Threading;
using System.Threading.Tasks;
using MeterReadingsBot.Models.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace MeterReadingsBot.Controllers
{
    public class ConfigureWebhook : IHostedService
    {
        private readonly BotConfiguration _botConfig;
        private readonly ILogger<ConfigureWebhook> _logger;
        private readonly IServiceProvider _services;

        public ConfigureWebhook(ILogger<ConfigureWebhook> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _services = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _botConfig = configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
            var webhookAddress = @$"{_botConfig.HostAddress}/bot/{_botConfig.BotToken}";

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
    }
}
