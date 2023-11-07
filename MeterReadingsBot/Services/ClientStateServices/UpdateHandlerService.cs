using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MeterReadingsBot.Entities;
using MeterReadingsBot.Enums;
using MeterReadingsBot.Repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MeterReadingsBot.Services.ClientStateServices;

/// <summary>
///     Определяет сервис обработки входящих сообщений в бота.
/// </summary>
public class UpdateHandlerService : UserClientServiceBase, IUpdateHandler
{
    #region Data
    #region Fields
    private readonly ILogger<UpdateHandlerService> _logger;
    private readonly IUserClientRepository _userClientRepository;

    private readonly IStartUserClientRepository _startUserClientRepository;
    private readonly WaterReadingsUserClientService _waterReadingsUserClientService;
    private readonly AdminUserClientService _adminUserClientService;
    #endregion
    #endregion

    #region .ctor
    /// <summary>
    ///     Инициализирует новый экземпляр типа <see cref="UpdateHandlerService" />
    /// </summary>
    /// <param name="botClient">Бот.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="userClientRepository">Репозиторий всех клиентов.</param>
    /// <param name="startUserClientRepository">Репозиторий стартовых клиентов.</param>
    /// <param name="waterReadingsUserClientService">Сервис клиентов передачи показаний. </param>
    /// <param name="adminUserClientService">Сервис суперадминов.</param>
    /// <exception cref="ArgumentNullException">Если один из аргументов не задан.</exception>
    public UpdateHandlerService(ITelegramBotClient botClient,
        ILogger<UpdateHandlerService> logger,
        IUserClientRepository userClientRepository,
        IStartUserClientRepository startUserClientRepository,
        WaterReadingsUserClientService waterReadingsUserClientService,
        AdminUserClientService adminUserClientService) : base(startUserClientRepository, botClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userClientRepository = userClientRepository ?? throw new ArgumentNullException(nameof(userClientRepository));
        _startUserClientRepository = startUserClientRepository ?? throw new ArgumentNullException(nameof(startUserClientRepository));
        _waterReadingsUserClientService = waterReadingsUserClientService ?? throw new ArgumentNullException(nameof(waterReadingsUserClientService));
        _adminUserClientService = adminUserClientService ?? throw new ArgumentNullException(nameof(adminUserClientService));
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
            { MyChatMember: { } myChatMember } => BotOnMyChatMemberReceived(myChatMember),
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

    private Task BotOnMyChatMemberReceived(ChatMemberUpdated myChatMember)
    {
        var chatId = myChatMember.Chat.Id;
        if (myChatMember.NewChatMember.Status == ChatMemberStatus.Kicked)
        {
            var clients = _userClientRepository.GetAllBy(chatId);
            foreach (var client in clients)
            {
                _userClientRepository.Remove(client);
            }
        }
        if (myChatMember.NewChatMember.Status == ChatMemberStatus.Member)
        {
            InitializeUsers(chatId);
        }
        return Task.CompletedTask;
    }
    #endregion

    #region Private
    private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
    {
       if (message.Type != MessageType.Text)
            return;
       var chatId = message.Chat.Id;
       var chatMessage = message.Text.Split(' ').First();
       if (chatMessage == "/start")
       {
           InitializeUsers(chatId);
           return;
       }
       if (chatMessage == MainMenuAnswer)
       {
           ResetStartUser(chatId);
       }
       var startState = GetStartUserState(chatId);
       _logger.LogInformation($"Receive message type: {message.Type}");
        var action = startState switch
        {
            UserClientState.Start => GetDefaultTaskMessage(message, cancellationToken),
            UserClientState.WaterReadings => _waterReadingsUserClientService.GetUserTaskMessage(message, cancellationToken),
            UserClientState.AdminUser => _adminUserClientService.GetUserTaskMessage(message, cancellationToken),
            _ => throw new ArgumentOutOfRangeException()
        };

        var sentMessage = await action;
        _logger.LogInformation($"The message was sent with id: {sentMessage.MessageId}");

    }

    private void InitializeUsers(long chatId)
    {
        if (_userClientRepository.GetAllBy(chatId).Count == 0)
        {
            _userClientRepository.Add(new StartUserClient(chatId)); // TODO фабрика создания всех видов клиентов и последующее добавление в репозитории.
            _userClientRepository.Add(new WaterReadingsUserClient(chatId));
        }
    }

    private void ResetStartUser(long chatId)
    {
        var startUser = _startUserClientRepository.FindBy(chatId);
        startUser.SetStateToStartState();
        _startUserClientRepository.Update(startUser);
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
            "/admin" => _adminUserClientService.GetStartUserTaskMessageAsync(message, cancellationToken),
            "/sendreadings" => _waterReadingsUserClientService.GetStartUserTaskMessageAsync(message, cancellationToken),
            "/help" => HelpMessage(message, cancellationToken),
            "/menu" => MenuMessage(message, cancellationToken),
            "/cancel" => MenuMessage(message, cancellationToken),
            _ => Usage(message, cancellationToken)
        };
    }

    private UserClientState GetStartUserState(long chatId)
    {
        var client = _startUserClientRepository.FindBy(chatId);
        if (client == null)
        {
            InitializeUsers(chatId);
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
                             "/admin - админские штучки :)\n" +
                             "Ошибки и пожелания - @KeST3107";

        return await TelegramBotClient.SendTextMessageAsync(
            message.Chat.Id,
            usage,
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: cancellationToken);
    }

    private async Task<Message> MenuMessage(Message message, CancellationToken cancellationToken)
    {
        const string usage = "Отправляй команду /sendreadings :)\n";

        return await TelegramBotClient.SendTextMessageAsync(
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
