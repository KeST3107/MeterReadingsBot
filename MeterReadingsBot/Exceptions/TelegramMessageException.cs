using System;

namespace MeterReadingsBot.Exceptions;

/// <summary>
/// Представляет ошибку, которая возникает в момент взаимодействия с клиентом посредством телеграма.
/// </summary>
public class TelegramMessageException : Exception
{
    /// <summary>
    /// Инициализирует новый экземпляр <see cref="TelegramMessageException"/>.
    /// </summary>
    /// <param name="message">Сообщение ошибки.</param>
    /// <param name="exception">Ошибка.</param>
    public TelegramMessageException(string message, Exception exception) : base(message, exception)
    {

    }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="TelegramMessageException"/>.
    /// </summary>
    /// <param name="message">Сообщение ошибки.</param>
    public TelegramMessageException(string message) : base(message)
    {

    }
}
