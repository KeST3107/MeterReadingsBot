﻿using System;

namespace MeterReadingsBot.Entities;

/// <summary>
/// Определяет базового клиента.
/// </summary>
public abstract class UserClientBase
{
    #region .ctor
    /// <summary>
    /// Инициализирует новый экземпляр типа <see cref="UserClientBase" />
    /// </summary>
    /// <param name="chatId">Идентификатор чата.</param>
    public UserClientBase(long chatId)
    {
        ChatId = chatId;
        Id = Guid.NewGuid();
        UpdateLastMessage();
    }
    protected UserClientBase()
    {

    }
    #endregion

    #region Properties
    /// <summary>
    /// Возвращает идентификатор чата.
    /// </summary>
    public long ChatId { get; private set; }

    /// <summary>
    /// Возвращает уникальный идентификатор.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Возвращает время последнего сообщения.
    /// </summary>
    public DateTime TimeLastMessage { get; private set; }
    #endregion

    #region Protected
    /// <summary>
    /// Обновляет время последнего сообщения.
    /// </summary>
    protected void UpdateLastMessage()
    {
        TimeLastMessage = DateTime.UtcNow;
    }
    #endregion
}
