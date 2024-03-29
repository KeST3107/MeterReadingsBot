﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MeterReadingsBot.Properties;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace MeterReadingsBot.Abstract;

/// <summary>
/// Определяет сервис получения новых данных из бота.
/// </summary>
/// <typeparam name="TUpdateHandler">Обработчик входящих сообщений.</typeparam>
public abstract class ReceiverServiceBase<TUpdateHandler> : IReceiverService
    where TUpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUpdateHandler _updateHandlerService;
    private readonly ILogger<ReceiverServiceBase<TUpdateHandler>> _logger;

    internal ReceiverServiceBase(
        ITelegramBotClient botClient,
        TUpdateHandler updateHandlerService,
        ILogger<ReceiverServiceBase<TUpdateHandler>> logger)
    {
        _botClient = botClient;
        _updateHandlerService = updateHandlerService;
        _logger = logger;
    }

    /// <summary>
    /// Отправляет новые входящие сообщения в обработчик.
    /// </summary>
    /// <param name="stoppingToken">Токен отмены.</param>
    /// <returns>Задача.</returns>
    public async Task ReceiveAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions()
        {
            AllowedUpdates = Array.Empty<UpdateType>(),
            ThrowPendingUpdates = true,
        };

        var me = await _botClient.GetMeAsync(stoppingToken);
        _logger.LogInformation(Resources.StrFmtInfoStartReceiveingUpdates, me.Username);

        // Start receiving updates
        await _botClient.ReceiveAsync(
            updateHandler: _updateHandlerService,
            receiverOptions: receiverOptions,
            cancellationToken: stoppingToken);
    }
}
