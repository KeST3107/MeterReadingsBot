using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MeterReadingsBot.Interfaces;
using MeterReadingsBot.Repositories;
using MeterReadingsBot.Settings;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace MeterReadingsBot.Services;

/// <summary>
///     Определяет сервис массовой рассылки.
/// </summary>
public class PromotionService : IPromotionService
{
    #region Data
    #region Fields
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<PromotionService> _logger;
    private readonly AdminUserSettings _settings;
    private readonly IStartUserClientRepository _startUserClientRepository;
    #endregion
    #endregion

    #region .ctor
    /// <summary>
    ///     Инициализирует новый экземпляр типа <see cref="PromotionService" />.
    /// </summary>
    /// <param name="botClient">Телеграм бот.</param>
    /// <param name="startUserClientRepository">Репозиторий стартовых клиентов.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="settings">Настройки.</param>
    /// <exception cref="ArgumentNullException">Если один из параметров не задан.</exception>
    public PromotionService(ITelegramBotClient botClient, IStartUserClientRepository startUserClientRepository, ILogger<PromotionService> logger, AdminUserSettings settings)
    {
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        _startUserClientRepository = startUserClientRepository ?? throw new ArgumentNullException(nameof(startUserClientRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }
    #endregion

    #region IPromotionService members
    /// <inheritdoc />
    public void StartPromotion(string message, CancellationToken cancellationToken, string promotionName = null)
    {
        var userIds = _startUserClientRepository.GetAll().Select(x=> x.ChatId).ToList();
        var countUsers = userIds.Count;
        var task = new Task(() =>
        {
            _logger.LogInformation("Запущена рассылка:{PromotionName} клиентам в количестве: {CountUsers}.", promotionName, countUsers);
            foreach (var id in userIds)
            {
                try
                {
                    _botClient.SendTextMessageAsync(id, message, cancellationToken: cancellationToken);
                }
                catch (Exception e)
                {
                    countUsers--;
                    _logger.LogError(e, "Рассылка пользователю {UserChatId} закончилась с ошибкой.", id);
                    throw;
                }
                Thread.Sleep(50);
            }
            _logger.LogInformation("Рассылка: {PromotionName} удачно дошла до {CountUsers} клиентов.",promotionName, countUsers);
            _botClient.SendTextMessageAsync(_settings.AdminChatId, $"Рассылка:{promotionName} удачно дошла до {countUsers} клиентов.", cancellationToken: cancellationToken);
        });
        task.Start();
    }
    #endregion
}
