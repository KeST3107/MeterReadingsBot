using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MeterReadingsBot.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace MeterReadingsBot.Services
{
    public class HandleUpdateService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<HandleUpdateService> _logger;
        private readonly IMessageHandler _messageHandler;

        public HandleUpdateService(ITelegramBotClient botClient, ILogger<HandleUpdateService> logger, IMessageHandler _messageHandler)
        {
            _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._messageHandler = _messageHandler ?? throw new ArgumentNullException(nameof(_messageHandler));
        }

        public async Task EchoAsync(Update update)
        {
            var handler = update.Type switch
            {
                // UpdateType.Unknown:
                // UpdateType.ChannelPost:
                // UpdateType.EditedChannelPost:
                // UpdateType.ShippingQuery:
                // UpdateType.PreCheckoutQuery:
                // UpdateType.Poll:
                UpdateType.Message => BotOnMessageReceived(update.Message),
                UpdateType.EditedMessage => BotOnMessageReceived(update.EditedMessage),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(update.CallbackQuery),
                UpdateType.InlineQuery => BotOnInlineQueryReceived(update.InlineQuery),
                UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(update.ChosenInlineResult),
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

        private async Task BotOnMessageReceived(Message message)
        {
            _logger.LogInformation($"Receive message type: {message.Type}");
            if (message.Type != MessageType.Text)
                return;

            var action = message.Text.Split(' ').First() switch
            {
                //"/help"     => SendHelpMessage(_botClient, message),
                "/sendWaterReadings" => WaterReadings(_botClient, message),
                //"/inline"   => SendInlineKeyboard(_botClient, message),
                //"/keyboard" => SendReplyKeyboard(_botClient, message),
                //"/remove"   => RemoveKeyboard(_botClient, message),
                //"/photo"    => SendFile(_botClient, message),
                //"/request"  => RequestContactAndLocation(_botClient, message),
                _ => Usage(_botClient, message)
            };
            var sentMessage = await action;
            _logger.LogInformation($"The message was sent with id: {sentMessage.MessageId}");

            // Send inline keyboard
            // You can process responses in BotOnCallbackQueryReceived handler
            static async Task<Message> SendInlineKeyboard(ITelegramBotClient bot, Message message)
            {
                await bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                // Simulate longer running task
                await Task.Delay(500);

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    // first row
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("11", "11"),
                        InlineKeyboardButton.WithCallbackData("12", "12")
                    },
                    // second row
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("21", $"Delete {message.MessageId + 1}"),
                        InlineKeyboardButton.WithCallbackData("22", "22")
                    }
                });

                return await bot.SendTextMessageAsync(message.Chat.Id,
                    "Choose",
                    replyMarkup: inlineKeyboard);
            }

            static async Task<Message> SendReplyKeyboard(ITelegramBotClient bot, Message message)
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

            static async Task<Message> RemoveKeyboard(ITelegramBotClient bot, Message message)
            {
                return await bot.SendTextMessageAsync(message.Chat.Id,
                    "Removing keyboard",
                    replyMarkup: new ReplyKeyboardRemove());
            }

            static async Task<Message> SendFile(ITelegramBotClient bot, Message message)
            {
                await bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                const string filePath = @"Files/tux.png";
                using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

                return await bot.SendPhotoAsync(message.Chat.Id,
                    new InputOnlineFile(fileStream, fileName),
                    "Nice Picture");
            }

            static async Task<Message> RequestContactAndLocation(ITelegramBotClient bot, Message message)
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

            async Task<Message> Usage(ITelegramBotClient bot, Message message)
            {
                _logger.LogInformation("Начало обработки сообщения с ID: {MessageId}.",message.MessageId);
                string usage = string.Empty;
                try
                {
                    usage = await _messageHandler.Handle(message);
                }
                catch (Exception e)
                {
                    _logger.LogError(e,"Обработка сообщения с ID: {MessageId} произошла с ошибкой.",message.MessageId);
                    return new Message();
                }
                _logger.LogInformation("Обработка сообщения с ID: {MessageId} прошла успешно.",message.MessageId);
                return await bot.SendTextMessageAsync(message.Chat.Id,
                    usage,
                    replyMarkup: new ReplyKeyboardRemove());
            }
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

        private async Task<Message> WaterReadings(ITelegramBotClient bot, Message message)
        {
            var helpMessage = "Чтобы передать показания введите данные в формате:\n" +
                              "200999999 150 200\n" +
                              "200999999 - Лицевой счет\n" +
                              "150 - Показания холодной воды\n" +
                              "200 - Показания горячей воды\n" +
                              "Показания отправляются в ОК и ТС, а также в Сев ДТВУ-4";

            return await bot.SendTextMessageAsync(message.Chat.Id,
                helpMessage);
        }

        // Process Inline Keyboard callback data
        private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
        {
            await _botClient.AnswerCallbackQueryAsync(
                callbackQuery.Id,
                $"Received {callbackQuery.Data}");

            await _botClient.SendTextMessageAsync(
                callbackQuery.Message.Chat.Id,
                $"Received {callbackQuery.Data}");
        }

        private Task UnknownUpdateHandlerAsync(Update update)
        {
            _logger.LogInformation($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }

        public Task HandleErrorAsync(Exception exception)
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

        #region Inline Mode

        private async Task BotOnInlineQueryReceived(InlineQuery inlineQuery)
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

        private Task BotOnChosenInlineResultReceived(ChosenInlineResult chosenInlineResult)
        {
            _logger.LogInformation($"Received inline result: {chosenInlineResult.ResultId}");
            return Task.CompletedTask;
        }

        #endregion
    }
}
