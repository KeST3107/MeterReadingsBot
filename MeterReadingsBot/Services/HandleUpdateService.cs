using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace MeterReadingsBot.Services
{
    using MeterReadingsBot.Interfaces;
    using MeterReadingsBot.Models;
    using Telegram.Bot;

    public class HandleUpdateService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<HandleUpdateService> _logger;
        private readonly IEmailService _emailService;
        private readonly IWaterReadingsService _waterReadingsService;

        public HandleUpdateService(ITelegramBotClient botClient, ILogger<HandleUpdateService> logger, IWaterReadingsService waterReadingsService, IEmailService emailService)
        {
            _botClient = botClient;
            _logger = logger;
            _emailService = emailService;
            _waterReadingsService = waterReadingsService;
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
                        InlineKeyboardButton.WithCallbackData("12", "12"),
                    },
                    // second row
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("21", $"Delete {message.MessageId + 1}"),
                        InlineKeyboardButton.WithCallbackData("22", "22"),
                    },
                });

                return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                    text: "Choose",
                    replyMarkup: inlineKeyboard);
            }

            static async Task<Message> SendReplyKeyboard(ITelegramBotClient bot, Message message)
            {
                var replyKeyboardMarkup = new ReplyKeyboardMarkup(
                    new KeyboardButton[][]
                    {
                        new KeyboardButton[] { "11", "12" },
                        new KeyboardButton[] { "21", "22" },
                    })
                {
                    ResizeKeyboard = true
                };

                return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                    text: "Choose",
                    replyMarkup: replyKeyboardMarkup);
            }

            static async Task<Message> RemoveKeyboard(ITelegramBotClient bot, Message message)
            {
                return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                    text: "Removing keyboard",
                    replyMarkup: new ReplyKeyboardRemove());
            }

            static async Task<Message> SendFile(ITelegramBotClient bot, Message message)
            {
                await bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                const string filePath = @"Files/tux.png";
                using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

                return await bot.SendPhotoAsync(chatId: message.Chat.Id,
                    photo: new InputOnlineFile(fileStream, fileName),
                    caption: "Nice Picture");
            }

            static async Task<Message> RequestContactAndLocation(ITelegramBotClient bot, Message message)
            {
                var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]
                {
                    KeyboardButton.WithRequestLocation("Location"),
                    KeyboardButton.WithRequestContact("Contact"),
                });

                return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                    text: "Who or Where are you?",
                    replyMarkup: RequestReplyKeyboard);
            }

            async Task<Message> Usage(ITelegramBotClient bot, Message message)
            {
                string usage = null;

                var splitMessage = message.Text.Split();

                if (splitMessage.Length < 3)
                {
                    usage = "При вводе данных что-то пропустили!\n";
                }

                if (splitMessage[0].Length < 9)
                {
                    usage = usage + "Неправильный лицевой счет!\n";
                }

                if (splitMessage.Length > 2 && splitMessage[0].Length == 9)
                {
                    int.TryParse(splitMessage[0], out var personalNumber);
                    int.TryParse(splitMessage[1], out var coldWater);
                    int.TryParse(splitMessage[2], out var hotWater);
                    var client = new Client
                    {
                        PersonalNumber = personalNumber,
                        ColdWater = coldWater,
                        HotWater = hotWater
                    };
                    var clientInfo = _waterReadingsService.GetClientInfo(client.PersonalNumber).Result;

                    if (clientInfo == null) usage = usage + "Номер не найден\n";
                    if (usage == null)
                    {
                        client.Address = clientInfo.Address;
                        client.FullName = clientInfo.FullName;
                        await _waterReadingsService.SendReadingsAsync(client.PersonalNumber, client.HotWater);
                        await _emailService.SendReadingsAsync(client.Address, client.ColdWater, client.HotWater);
                        return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                            text: $"{client.FullName}\n" +
                                  $"{client.Address}\n" +
                                  $"Холодная вода: {coldWater}\n" +
                                  $"Горячая вода: {hotWater}\n" +
                                  $"Показания переданы!",
                            replyMarkup: new ReplyKeyboardRemove());
                    }
                }

                return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                    text: usage,
                    replyMarkup: new ReplyKeyboardRemove());
            }
        }

        async Task<Message> SendHelpMessage(ITelegramBotClient bot, Message message)
        {
            string helpMessage = "Чтобы пользоваться ботом нужно выбрать соотетствующий раздел:\n" +
                                 "мод - предназначен для\n" +
                                 "мод - предназначен для\n" +
                                 "мод - предназначен для\n" +
                                 "мод - предназначен для\n" +
                                 "И передать данные в указанном формате!";

            return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                text: helpMessage);
        }

        async Task<Message> WaterReadings(ITelegramBotClient bot, Message message)
        {
            string helpMessage = "Чтобы передать показания введите данные в формате:\n" +
                                 "200999999 150 200\n" +
                                 "200999999 - Лицевой счет\n" +
                                 "150 - Показания холодной воды\n" +
                                 "200 - Показания горячей воды\n" +
                                 "Показания отправляются в ОК и ТС, а также в Сев ДТВУ-4";

            return await bot.SendTextMessageAsync(chatId: message.Chat.Id,
                text: helpMessage);
        }

        // Process Inline Keyboard callback data
        private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
        {
            await _botClient.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: $"Received {callbackQuery.Data}");

            await _botClient.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: $"Received {callbackQuery.Data}");
        }

        #region Inline Mode

        private async Task BotOnInlineQueryReceived(InlineQuery inlineQuery)
        {
            _logger.LogInformation($"Received inline query from: {inlineQuery.From.Id}");

            InlineQueryResultBase[] results =
            {
                // displayed result
                new InlineQueryResultArticle(
                    id: "3",
                    title: "TgBots",
                    inputMessageContent: new InputTextMessageContent(
                        "hello"
                    )
                )
            };

            await _botClient.AnswerInlineQueryAsync(inlineQueryId: inlineQuery.Id,
                results: results,
                isPersonal: true,
                cacheTime: 0);
        }

        private Task BotOnChosenInlineResultReceived(ChosenInlineResult chosenInlineResult)
        {
            _logger.LogInformation($"Received inline result: {chosenInlineResult.ResultId}");
            return Task.CompletedTask;
        }

        #endregion

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
    }
}
