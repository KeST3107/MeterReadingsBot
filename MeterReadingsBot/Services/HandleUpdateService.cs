using System;
using System.IO;
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
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace MeterReadingsBot.Services;

/// <summary>
/// Определяет сервис обработки входящих сообщений в бота.
/// </summary>
public class HandleUpdateService : UserClientServiceBase
{
    #region Data
    #region Fields
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<HandleUpdateService> _logger;
    private readonly IStartUserClientRepository _startUserClientRepository;
    private readonly IWaterReadingsUserClientService _waterReadingsUserClientService;
    #endregion
    #endregion

    #region .ctor
    /// <summary>
    /// Инициализирует новый экземпляр типа <see cref="HandleUpdateService" />
    /// </summary>
    /// <param name="botClient">Бот.</param>
    /// <param name="logger">Логгер.</param>
    /// <param name="startUserClientRepository">Репозиторий стартовых клиентов.</param>
    /// <param name="waterReadingsUserClientService">Сервис клиентов передачи показаний. </param>
    /// <exception cref="ArgumentNullException">Если один из аргументов не задан.</exception>
    public HandleUpdateService(ITelegramBotClient botClient,
        ILogger<HandleUpdateService> logger,
        IStartUserClientRepository startUserClientRepository,
        IWaterReadingsUserClientService waterReadingsUserClientService) : base(startUserClientRepository)
    {
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _startUserClientRepository = startUserClientRepository ?? throw new ArgumentNullException(nameof(startUserClientRepository));
        _waterReadingsUserClientService = waterReadingsUserClientService ?? throw new ArgumentNullException(nameof(waterReadingsUserClientService));
    }
    #endregion

    #region Public
    /// <summary>
    /// Выполняет обработку принятого сообщения.
    /// </summary>
    /// <param name="update">Сообщение.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task EchoAsync(Update update, CancellationToken cancellationToken)
    {
        var handler = update.Type switch
        {
            // UpdateType.Unknown:
            // UpdateType.ChannelPost:
            // UpdateType.EditedChannelPost:
            // UpdateType.ShippingQuery:
            // UpdateType.PreCheckoutQuery:
            // UpdateType.Poll:
            UpdateType.Message => BotOnMessageReceived(update.Message, cancellationToken),
            //UpdateType.EditedMessage => BotOnMessageReceived(update.EditedMessage, cancellationToken),
            //UpdateType.CallbackQuery => BotOnCallbackQueryReceived(update.CallbackQuery, cancellationToken),
            //UpdateType.InlineQuery => BotOnInlineQueryReceived(update.InlineQuery, cancellationToken),
            //UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(update.ChosenInlineResult, cancellationToken),
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

    private Task HandleErrorAsync(Exception exception)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException =>
                $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation(ErrorMessage);
        return Task.CompletedTask;
    }
    #endregion

    #region Private
    // Process Inline Keyboard callback data
    private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        await _botClient.AnswerCallbackQueryAsync(
            callbackQuery.Id,
            $"Received {callbackQuery.Data}");

        await _botClient.SendTextMessageAsync(
            callbackQuery.Message.Chat.Id,
            $"Received {callbackQuery.Data}");
    }

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
        if (client == null) _startUserClientRepository.Add(new StartUserClient(chatId));
        return client == null ? UserClientState.Start : client.State;
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

    private static async Task<Message> RemoveKeyboard(ITelegramBotClient bot, Message message, CancellationToken cancellationToken)
    {
        return await bot.SendTextMessageAsync(message.Chat.Id,
            "Removing keyboard",
            replyMarkup: new ReplyKeyboardRemove());
    }
    private static async Task<Message> RequestContactAndLocation(ITelegramBotClient bot, Message message, CancellationToken cancellationToken)
    {
        var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]
        {
            KeyboardButton.WithRequestLocation("Location"),
            KeyboardButton.WithRequestContact("Contact")
        });

        return await bot.SendTextMessageAsync(message.Chat.Id,
            "Who or Where are you?",
            replyMarkup: RequestReplyKeyboard);
    }
    private static async Task<Message> SendFile(ITelegramBotClient bot, Message message, CancellationToken cancellationToken)
    {
        await bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

        const string filePath = @"Files/tux.png";
        using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

        return await bot.SendPhotoAsync(message.Chat.Id,
            new InputOnlineFile(fileStream, fileName),
            "Nice Picture");
    }

    private async Task<Message> SendHelpMessage(ITelegramBotClient bot, Message message)
    {
        var helpMessage = "Чтобы пользоваться ботом нужно выбрать соотетствующий раздел:\n" +
                          "мод - предназначен для\n" +
                          "мод - предназначен для\n" +
                          "мод - предназначен для\n" +
                          "мод - предназначен для\n" +
                          "И передать данные в указанном формате!";

        return await bot.SendTextMessageAsync(message.Chat.Id,
            helpMessage);
    }
    private static async Task<Message> SendReplyKeyboard(ITelegramBotClient bot, Message message, CancellationToken cancellationToken)
    {
        var replyKeyboardMarkup = new ReplyKeyboardMarkup(
            new[]
            {
                new KeyboardButton[] { "11", "12" },
                new KeyboardButton[] { "21", "22" }
            })
        {
            ResizeKeyboard = true
        };

        return await bot.SendTextMessageAsync(message.Chat.Id,
            "Choose",
            replyMarkup: replyKeyboardMarkup);
    }
    private static async Task<Message> StartInlineQuery(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        InlineKeyboardMarkup inlineKeyboard = new(
            InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Inline Mode"));

        return await botClient.SendTextMessageAsync(
            message.Chat.Id,
            "Press the button to start Inline Query",
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        _logger.LogInformation($"Пока не могу обрабатывать данный тип данных 😞: {update.Type}");
        return Task.CompletedTask;
    }
    #endregion

    #region Inline Mode
    private async Task BotOnInlineQueryReceived(InlineQuery inlineQuery, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Received inline query from: {inlineQuery.From.Id}");

        InlineQueryResultBase[] results =
        {
            // displayed result
            new InlineQueryResultArticle(
                "3",
                "TgBots",
                new InputTextMessageContent(
                    "hello"
                )
            )
        };

        await _botClient.AnswerInlineQueryAsync(inlineQuery.Id,
            results,
            isPersonal: true,
            cacheTime: 0);
    }

    private Task BotOnChosenInlineResultReceived(ChosenInlineResult chosenInlineResult, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Received inline result: {chosenInlineResult.ResultId}");
        return Task.CompletedTask;
    }
    #endregion
}
