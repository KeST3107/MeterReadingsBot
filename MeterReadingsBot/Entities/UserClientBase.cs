using System;
using System.ComponentModel.DataAnnotations;

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
    protected UserClientBase(long chatId)
    {
        ChatId = chatId;
        Id = Guid.NewGuid();
        UpdateLastMessage();
    }
    #endregion

    #region Properties
    /// <summary>
    /// Возвращает идентификатор чата.
    /// </summary>
    public long ChatId { get; }

    /// <summary>
    /// Возвращает уникальный идентификатор.
    /// </summary>
    [Key]
    public Guid Id { get; }

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
