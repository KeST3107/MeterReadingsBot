﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MeterReadingsBot.Entities;
using MeterReadingsBot.Enums;
using MeterReadingsBot.Exceptions;
using MeterReadingsBot.Interfaces;
using MeterReadingsBot.Repositories;
using MeterReadingsBot.Settings;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MeterReadingsBot.Services.ClientStateServices;

/// <summary>
///     Определяет сервис клиентов передачи показаний.
/// </summary>
public class WaterReadingsUserClientService : UserClientServiceBase, IUserClientService
{
    #region Data
    #region Fields
    private readonly WaterReadingsServiceSettings _settings;
    private readonly IWaterReadingsClientRepository _waterReadingsClientRepository;
    private readonly IWaterReadingsService _waterReadingsService;
    #endregion
    #endregion

    #region .ctor
    /// <summary>
    ///     Инициализирует новый экземпляр типа <see cref="WaterReadingsUserClientService" />
    /// </summary>
    /// <param name="botClient">Бот.</param>
    /// <param name="settings">Настройки передачи показаний.</param>
    /// <param name="waterReadingsClientRepository">Репозиторий клиентов передачи показаний.</param>
    /// <param name="waterReadingsService">Сервис передачи показаний.</param>
    /// <param name="startUserClientRepository">Репозиторий стартовых клиентов.</param>
    /// <exception cref="ArgumentNullException">Если один из аргументов не задан.</exception>
    public WaterReadingsUserClientService(ITelegramBotClient botClient,
        WaterReadingsServiceSettings settings,
        IWaterReadingsClientRepository waterReadingsClientRepository,
        IWaterReadingsService waterReadingsService,
        IStartUserClientRepository startUserClientRepository) : base(startUserClientRepository, botClient)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _waterReadingsClientRepository = waterReadingsClientRepository ?? throw new ArgumentNullException(nameof(waterReadingsClientRepository));
        _waterReadingsService = waterReadingsService ?? throw new ArgumentNullException(nameof(waterReadingsService));
    }
    #endregion

    #region IUserClientService members
    /// <inheritdoc />
    public async Task<Message> GetStartUserTaskMessageAsync(Message message, CancellationToken cancellationToken)
    {
        var chatMessage = message.Text.Split(' ').First();
        if (chatMessage == "/sendreadings" && (_settings.DateFrom > DateTime.Now.Day || _settings.DateTo < DateTime.Now.Day))
            throw new TelegramMessageException($"Показания можно передавать только с {_settings.DateFrom} по {_settings.DateTo} число.");
        var chatId = message.Chat.Id;
        var userClientModel = _waterReadingsClientRepository.FindBy(chatId) ?? _waterReadingsClientRepository.Add(new WaterReadingsUserClient(chatId));
        userClientModel.WaterReadingsState = WaterReadingsState.PersonalNumber;
        var personalNumbers = userClientModel.PersonalNumbers;
        _waterReadingsClientRepository.Update(userClientModel);
        SetStartUserToWaterReadings(chatId);
        if (personalNumbers.Any())
        {
            var keyboardButton = new KeyboardButton[personalNumbers.Count];
            for (var i = 0; i < personalNumbers.Count; i++) keyboardButton[i] = new KeyboardButton(personalNumbers[i]);
            var replyKeyboardMarkup = new ReplyKeyboardMarkup(
                new[]
                {
                    keyboardButton
                })
            {
                ResizeKeyboard = true
            };
            return await TelegramBotClient.SendTextMessageAsync(chatId,
                "Введите лицевой счет ОК и ТС, например 299999999\n" +
                "Ваши введенные ранее лицевые счета:",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
        }
        return await TelegramBotClient.SendTextMessageAsync(chatId, "Введите лицевой счет, например 299999999", cancellationToken: cancellationToken);
    }
    /// <inheritdoc />
    public Task<Message> GetUserTaskMessage(Message message, CancellationToken cancellationToken)
    {
        var chatMessage = message.Text.Split(' ').First();
        if (chatMessage == ReturnAnswer || chatMessage == MainMenuAnswer) ResetUser(message.Chat.Id);
        var waterReadingsClient = _waterReadingsClientRepository.FindBy(message.Chat.Id);
        if (waterReadingsClient == null) return Usage(message, cancellationToken);
        return waterReadingsClient.WaterReadingsState switch
        {
            WaterReadingsState.Start => GetStartUserTaskMessageAsync(message, cancellationToken),
            WaterReadingsState.PersonalNumber => GetClientInfo(waterReadingsClient, message, cancellationToken),
            WaterReadingsState.ConfirmClientInfo => ConfirmClientInfo(waterReadingsClient, message, cancellationToken),
            WaterReadingsState.ColdWaterBathroom => SaveWaterReadings(waterReadingsClient, message, cancellationToken),
            WaterReadingsState.HotWaterBathroom => SaveWaterReadings(waterReadingsClient, message, cancellationToken),
            WaterReadingsState.ColdWaterKitchen => SaveWaterReadings(waterReadingsClient, message, cancellationToken),
            WaterReadingsState.HotWaterKitchen => SaveWaterReadings(waterReadingsClient, message, cancellationToken),
            WaterReadingsState.ConfirmWaterReadings => SendWaterReadings(waterReadingsClient, message, cancellationToken),
            WaterReadingsState.ContinueSendWaterReadings => ContinueSendWaterReadings(waterReadingsClient, message, cancellationToken),
            _ => Usage(message, cancellationToken)
        };
    }
    #endregion

    #region Private
    private async Task<Message> ConfirmClientInfo(WaterReadingsUserClient userClient, Message message, CancellationToken cancellationToken)
    {
        string chatMessage;
        switch (message.Text.ToUpper())
        {
            case ConfirmationAnswer:
                chatMessage = "Введите показания холодной воды в санузле.";
                userClient.WaterReadingsState = WaterReadingsState.ColdWaterBathroom;
                break;
            case RejectionAnswer:
                chatMessage = "Введите номер лицевого счета заново.";
                userClient.WaterReadingsState = WaterReadingsState.PersonalNumber;
                break;
            default:
                throw new TelegramMessageException(ConfirmationAnswerErrorMessage);
        }
        _waterReadingsClientRepository.Update(userClient);
        return await TelegramBotClient.SendTextMessageAsync(message.Chat.Id,
            chatMessage,
            cancellationToken: cancellationToken,
            replyMarkup: new ReplyKeyboardRemove());
    }

    private async Task<Message> ContinueSendWaterReadings(WaterReadingsUserClient userClient, Message message, CancellationToken cancellationToken)
    {
        string chatMessage;
        switch (message.Text.ToUpper())
        {
            case ConfirmationAnswer:
                userClient.WaterReadingsState = WaterReadingsState.Start;
                _waterReadingsClientRepository.Update(userClient);
                return await GetStartUserTaskMessageAsync(message, cancellationToken);
            case RejectionAnswer:
                chatMessage = "Спасибо за переданные показания\n" +
                              "Чтобы подать показания заново: /sendreadings\n" +
                              "Вызвать основное меню: /help ";
                userClient.WaterReadingsState = WaterReadingsState.Start;
                _waterReadingsClientRepository.Update(userClient);
                SetStartUserToDefault(userClient.ChatId);
                break;
            default:
                throw new TelegramMessageException(ConfirmationAnswerErrorMessage);
        }
        return await TelegramBotClient.SendTextMessageAsync(message.Chat.Id,
            chatMessage,
            cancellationToken: cancellationToken,
            replyMarkup: new ReplyKeyboardRemove());
    }

    private async Task<Message> GetClientInfo(WaterReadingsUserClient userClient, Message message, CancellationToken cancellationToken)
    {
        var personalNumber = GetPersonnelNumber(message);
        var clientInfo = await _waterReadingsService.GetClientInfoAsync(personalNumber, cancellationToken);
        var chatMessage = $"По номеру: {personalNumber} найден клиент:\n" +
                          $"{clientInfo.FullName}\n" +
                          $"{clientInfo.Address}\n" +
                          "Все верно?";
        userClient.WaterReadingsState = WaterReadingsState.ConfirmClientInfo;
        userClient.UpdateTempClient(clientInfo);
        _waterReadingsClientRepository.Update(userClient);
        return await TelegramBotClient.SendTextMessageAsync(message.Chat.Id,
            chatMessage,
            replyMarkup: GetReplyKeyboard(),
            cancellationToken: cancellationToken);
    }

    private long GetPersonnelNumber(Message message)
    {
        var isConvertible = long.TryParse(message.Text, out var personalNumber);
        if (isConvertible is false || message.Text.Length != 13)
            throw new TelegramMessageException("Введено недопустимое значение.");
        return personalNumber;
    }

    private void ResetUser(long chatId)
    {
        var waterClient = _waterReadingsClientRepository.FindBy(chatId);
        waterClient.WaterReadingsState = WaterReadingsState.Start;
        _waterReadingsClientRepository.Update(waterClient);
    }

    private async Task<Message> SaveWaterReadings(WaterReadingsUserClient userClient, Message message, CancellationToken cancellationToken)
    {
        string chatMessage;
        var chatId = message.Chat.Id;
        var replyKeyboardMarkup = GetReplyKeyboard();
        if (message.Text == "-" && userClient.WaterReadingsState == WaterReadingsState.ColdWaterKitchen)
        {
            userClient.WaterReadingsState = WaterReadingsState.HotWaterKitchen;
            userClient.TempClient.ColdWaterKitchen = null;
            _waterReadingsClientRepository.Update(userClient);
            return await TelegramBotClient.SendTextMessageAsync(chatId,
                "Показания холодной воды на кухне в количестве: - сохранены\n" +
                "Введите показания горячей воды на кухне, если показания отсутствуют укажите '-'. ",
                cancellationToken: cancellationToken);
        }
        if (message.Text == "-" && userClient.WaterReadingsState == WaterReadingsState.HotWaterKitchen)
        {
            userClient.WaterReadingsState = WaterReadingsState.ConfirmWaterReadings;
            userClient.TempClient.ColdWaterKitchen = null;
            _waterReadingsClientRepository.Update(userClient);
            var clientInfo = userClient.TempClient;
            chatMessage = $"Холодная вода в санузле: {clientInfo.ColdWaterBathroom}\n" + // TODO Добавить адрес
                          $"Горячая вода в санузле: {clientInfo.HotWaterBathroom}\n" +
                          $"Холодная вода на кухне: {clientInfo.ColdWaterKitchen}\n" +
                          $"Горячая вода на кухне: {clientInfo.HotWaterKitchen}\n" +
                          "Данные указаны верно?";
            return await TelegramBotClient.SendTextMessageAsync(chatId, chatMessage, replyMarkup: replyKeyboardMarkup, cancellationToken: cancellationToken);
        }
        var isConvertible = int.TryParse(message.Text, out var waterReadings);
        if (isConvertible is false) return await TelegramBotClient.SendTextMessageAsync(message.Chat.Id, "Введено недопустимое значение.", cancellationToken: cancellationToken);

        switch (userClient.WaterReadingsState)
        {
            case WaterReadingsState.ColdWaterBathroom:
                userClient.TempClient.ColdWaterBathroom = waterReadings;
                userClient.WaterReadingsState = WaterReadingsState.HotWaterBathroom;
                chatMessage = $"Холодная вода в санузле: {waterReadings} сохранена\n" +
                              "Введите показания горячей воды в санузле. ";
                break;
            case WaterReadingsState.HotWaterBathroom:
                userClient.TempClient.HotWaterBathroom = waterReadings;
                userClient.WaterReadingsState = WaterReadingsState.ColdWaterKitchen;
                chatMessage = $"Горячая вода в санузле в количестве: {waterReadings} сохранена\n" +
                              "Введите показания холодной воды на кухне\n" +
                              "Если показания отсутствуют укажите '-'. ";
                break;
            case WaterReadingsState.ColdWaterKitchen:
                userClient.TempClient.ColdWaterKitchen = waterReadings;
                userClient.WaterReadingsState = WaterReadingsState.HotWaterKitchen;
                chatMessage = $"Холодная вода на кухне в количестве: {waterReadings} сохранена\n" +
                              "Введите показания горячей воды на кухне\n" +
                              "Если показания отсутствуют укажите '-'.";
                break;
            case WaterReadingsState.HotWaterKitchen:
                userClient.TempClient.HotWaterKitchen = waterReadings;
                userClient.WaterReadingsState = WaterReadingsState.ConfirmWaterReadings;
                chatMessage = $"Горячая вода на кухне в количестве: {waterReadings} сохранена.";
                break;
            default:
                return await TelegramBotClient.SendTextMessageAsync(chatId, "Введено недопустимое значение.", cancellationToken: cancellationToken);
        }
        if (userClient.WaterReadingsState == WaterReadingsState.ConfirmWaterReadings)
        {
            var clientInfo = userClient.TempClient;
            chatMessage = chatMessage +
                          $"\nХолодная вода в санузле: {clientInfo.ColdWaterBathroom}\n" +
                          $"Горячая вода в санузле: {clientInfo.HotWaterBathroom}\n" +
                          $"Холодная вода на кухне: {clientInfo.ColdWaterKitchen}\n" +
                          $"Горячая вода на кухне: {clientInfo.HotWaterKitchen}\n" +
                          "Данные указаны верно?";
            _waterReadingsClientRepository.Update(userClient);
            return await TelegramBotClient.SendTextMessageAsync(chatId, chatMessage, replyMarkup: replyKeyboardMarkup, cancellationToken: cancellationToken);
        }
        _waterReadingsClientRepository.Update(userClient);
        return await TelegramBotClient.SendTextMessageAsync(chatId, chatMessage, cancellationToken: cancellationToken);
    }

    private async Task<Message> SendWaterReadings(WaterReadingsUserClient userClient, Message message, CancellationToken cancellationToken)
    {
        string chatMessage;
        switch (message.Text.ToUpper())
        {
            case ConfirmationAnswer:
                try
                {
                    await _waterReadingsService.SendWaterReadingsOKiTSAsync(userClient.TempClient, cancellationToken);
                    await _waterReadingsService.SendWaterReadingsToGorvodokanalAsync(userClient.TempClient);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                chatMessage = "Показания переданы успешно!\n" +
                              "Передаем показания еще?";

                userClient.AddPersonnelNumber(userClient.TempClient.PersonalNumber);
                userClient.WaterReadingsState = WaterReadingsState.ContinueSendWaterReadings;
                _waterReadingsClientRepository.Update(userClient);
                break;
            case RejectionAnswer:
                chatMessage = "Информация не подтверждена.\n" +
                              "Введите показания холодной воды в санузле.";
                userClient.WaterReadingsState = WaterReadingsState.ColdWaterBathroom;
                _waterReadingsClientRepository.Update(userClient);
                return await TelegramBotClient.SendTextMessageAsync(message.Chat.Id, chatMessage, cancellationToken: cancellationToken, replyMarkup: new ReplyKeyboardRemove());
            default:
                throw new TelegramMessageException(ConfirmationAnswerErrorMessage);
        }
        return await TelegramBotClient.SendTextMessageAsync(message.Chat.Id, chatMessage, cancellationToken: cancellationToken, replyMarkup: GetReplyKeyboard());
    }
    #endregion
}
