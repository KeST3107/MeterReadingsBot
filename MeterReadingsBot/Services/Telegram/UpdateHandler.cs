using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MeterReadingsBot.Entities;
using MeterReadingsBot.Enums;
using MeterReadingsBot.Repositories;
using MeterReadingsBot.Services.ClientStateServices;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MeterReadingsBot.Services.Telegram;

/// <summary>
///     Определяет сервис обработки входящих сообщений в бота.
/// </summary>
public class UpdateHandler : UserClientServiceBase, IUpdateHandler
{
    #region Data
    #region Fields
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<UpdateHandler> _logger;
    private readonly IUserClientRepository _userClientRepository;

    private readonly IStartUserClientRepository _startUserClientRepository;
    private readonly IWaterReadingsUserClientService _waterReadingsUserClientService;
    #endregion
    #endregion

    #region .ctor
    /// <summary>
    ///     Инициализирует новый экземпляр типа <see cref="UpdateHandler" />
    /// </summary>
    /// <param name="botClient">Бот.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="userClientRepository">Репозиторий всех клиентов.</param>
    /// <param name="startUserClientRepository">Репозиторий стартовых клиентов.</param>
    /// <param name="waterReadingsUserClientService">Сервис клиентов передачи показаний. </param>
    /// <exception cref="ArgumentNullException">Если один из аргументов не задан.</exception>
    public UpdateHandler(ITelegramBotClient botClient,
        ILogger<UpdateHandler> logger,
        IUserClientRepository userClientRepository,
        IStartUserClientRepository startUserClientRepository,
        IWaterReadingsUserClientService waterReadingsUserClientService) : base(startUserClientRepository)
    {
        _botClient = botClient;
        _logger = logger;
        _userClientRepository = userClientRepository ?? throw new ArgumentNullException(nameof(userClientRepository));
        _startUserClientRepository = startUserClientRepository ?? throw new ArgumentNullException(nameof(startUserClientRepository));
        _waterReadingsUserClientService = waterReadingsUserClientService ?? throw new ArgumentNullException(nameof(waterReadingsUserClientService));
    }
    #endregion

    #region IUpdateHandler members
    /// <summary>
    /// Обрабатывает исключения.
    /// </summary>
    /// <param name="botClient">Бот.</param>
    /// <param name="exception">Исключение.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", errorMessage);

        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

    /// <summary>
    ///     Выполняет обработку принятого сообщения.
    /// </summary>
    /// <param name="update">Сообщение.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
    {
        var handler = update switch
        {
            // UpdateType.Unknown:
            // UpdateType.ChannelPost:
            // UpdateType.EditedChannelPost:
            // UpdateType.ShippingQuery:
            // UpdateType.PreCheckoutQuery:
            // UpdateType.Poll:
            { Message: { } message } => BotOnMessageReceived(message, cancellationToken),
            //{ EditedMessage: { } message }                 => BotOnMessageReceived(message, cancellationToken),
            //{ CallbackQuery: { } callbackQuery }           => BotOnCallbackQueryReceived(callbackQuery, cancellationToken),
            //{ InlineQuery: { } inlineQuery }               => BotOnInlineQueryReceived(inlineQuery, cancellationToken),
            //{ ChosenInlineResult: { } chosenInlineResult } => BotOnChosenInlineResultReceived(chosenInlineResult, cancellationToken),
            _ => UnknownUpdateHandlerAsync(update)
        };

        try
        {
            await handler;
        }
        catch (Exception exception)
        {
            await HandleErrorAsync(exception);
        }
    }
    #endregion

    #region Private
    private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
    {
        var startState = GetStartUserState(message.Chat.Id);
        _logger.LogInformation($"Receive message type: {message.Type}");
        if (message.Type != MessageType.Text)
            return;
        var action = startState switch
        {
            UserClientState.Start => GetDefaultTaskMessage(message, cancellationToken),
            UserClientState.WaterReadings => _waterReadingsUserClientService.GetWaterReadingsTaskMessage(message, cancellationToken)
        };

        var sentMessage = await action;
        _logger.LogInformation($"The message was sent with id: {sentMessage.MessageId}");

    }

    private Task<Message> GetDefaultTaskMessage(Message message, CancellationToken cancellationToken)
    {
        var chatMessage = message.Text.Split(' ').First();
        return chatMessage switch
        {
            /*"/help"      => SendHelpMessage(_botClient, message),
            "/sendWaterReadings" => WaterReadings(_botClient, message),
            "/inline"   => SendInlineKeyboard(_botClient, message),
            "/keyboard" => SendReplyKeyboard(_botClient, message),
            "/remove"   => RemoveKeyboard(_botClient, message),
            "/photo"    => SendFile(_botClient, message),
            "/request"  => RequestContactAndLocation(_botClient, message),
            _ => Usage(_botClient, message, cancellationToken)*/
            "/sendreadings" => _waterReadingsUserClientService.GetStartWaterReadingsTaskMessage(message, cancellationToken),
            "/help" => HelpMessage(message, cancellationToken),
            _ => Usage(_botClient, message, cancellationToken)
        };
    }

    private UserClientState GetStartUserState(long chatId)
    {
        var client = _startUserClientRepository.FindBy(chatId);
        if (client == null)
        {
            _userClientRepository.Add(new StartUserClient(chatId));
            _userClientRepository.Add(new WaterReadingsUserClient(chatId));
        }
        return client == null ? UserClientState.Start : client.State;
    }

    private Task HandleErrorAsync(Exception exception)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException =>
                $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation(errorMessage);
        return Task.CompletedTask;
    }


    private async Task<Message> HelpMessage(Message message, CancellationToken cancellationToken)
    {
        const string usage = "Команды бота:\n" +
                             "/help - отправляет сообщение с командами\n" +
                             "/sendreadings - запускает команду подачу показаний\n" +
                             "Ошибки и пожелания - @KeST3107";

        return await _botClient.SendTextMessageAsync(
            message.Chat.Id,
            usage,
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: cancellationToken);
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        _logger.LogInformation($"Пока не могу обрабатывать данный тип данных 😞: {update.Type}");
        return Task.CompletedTask;
    }
    #endregion
}
