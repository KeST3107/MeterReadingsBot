using System;
using System.Threading;
using System.Threading.Tasks;
using MeterReadingsBot.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MeterReadingsBot.Services.ClientStateServices;

/// <summary>
/// Определяет базовую логику взаимодействия с клиентами.
/// </summary>
public abstract class UserClientServiceBase
{
    #region Data
    #region Consts
    /// <summary>
    /// Подтверждающий ответ.
    /// </summary>
    protected const string ConfirmationAnswer = "ДА";
    /// <summary>
    /// Отклоняющий ответ.
    /// </summary>
    protected const string RejectionAnswer = "НЕТ";
    /// <summary>
    /// Отклоняющий ответ.
    /// </summary>
    protected const string ReturnAnswer = "/cancel";
    /// <summary>
    /// Отклоняющий ответ.
    /// </summary>
    protected const string MainMenuAnswer = "/menu";
    #endregion

    #region Fields
    private readonly IStartUserClientRepository _startUserClientRepository;
    /// <summary>
    /// Клиент телеграм бота.
    /// </summary>
    protected readonly ITelegramBotClient TelegramBotClient;
    #endregion
    #endregion

    #region .ctor
    /// <summary>
    /// Инициализирует новый экземпляр типа <see cref="UserClientServiceBase" />.
    /// </summary>
    /// <param name="startUserClientRepository">Репозиторий стартовых клиентов.</param>
    /// <param name="telegramBotClient">Клиент телеграм бота.</param>
    /// <exception cref="ArgumentNullException">Если <see cref="IStartUserClientRepository"/> не задан.</exception>
    public UserClientServiceBase(IStartUserClientRepository startUserClientRepository, ITelegramBotClient telegramBotClient)
    {
        _startUserClientRepository = startUserClientRepository ?? throw new ArgumentNullException(nameof(startUserClientRepository));
        TelegramBotClient = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));
    }
    #endregion

    #region Protected
    /// <summary>
    /// Возвращает клавиатуру с ответами "ДА", "НЕТ".
    /// </summary>
    /// <returns></returns>
    protected ReplyKeyboardMarkup GetReplyKeyboard()
    {
        return new ReplyKeyboardMarkup(
            new[]
            {
                new KeyboardButton[] { ConfirmationAnswer, RejectionAnswer }
            })
        {
            ResizeKeyboard = true
        };
    }

    /// <summary>
    /// Устанавливает начальное состояние стартового клиента.
    /// </summary>
    /// <param name="chatId">Идентификатор чата.</param>
    protected void SetStartUserToDefault(long chatId)
    {
        var startUserClient = _startUserClientRepository.FindBy(chatId);
        startUserClient?.SetStateToStartState();
        _startUserClientRepository.Update(startUserClient);
    }


    /// <summary>
    /// Устанавливает состояние передачи показаний стартового клиента.
    /// </summary>
    /// <param name="chatId">Идентификатор чата.</param>
    protected void SetStartUserToWaterReadings(long chatId)
    {
        var startUserClient = _startUserClientRepository.FindBy(chatId);
        startUserClient?.SetStateToWaterReadingsState();
        _startUserClientRepository.Update(startUserClient);
    }

    /// <summary>
    /// Устанавливает состояние админа стартового клиента.
    /// </summary>
    /// <param name="chatId">Идентификатор чата.</param>
    protected void SetStartUserToAdminUser(long chatId)
    {
        var startUserClient = _startUserClientRepository.FindBy(chatId);
        startUserClient?.SetStateToAdminUserState();
        _startUserClientRepository.Update(startUserClient);
    }

    /// <summary>
    /// Возвращает базовое сообщение, если сообщение не соответствует.
    /// </summary>
    /// <param name="message">Сообщение клиента.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Задача сообщения.</returns>
    protected async Task<Message> Usage(Message message, CancellationToken cancellationToken)
    {
        const string usage = "Я тебя не понимаю 😞\n" +
                             "Попробуй эту команду /help";

        return await TelegramBotClient.SendTextMessageAsync(
            message.Chat.Id,
            usage,
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: cancellationToken);
    }
    #endregion
}
