using System;

namespace MeterReadingsBot.Entities;

/// <summary>
/// Представляет сущность временного клиента.
/// </summary>
public class Client
{
    #region Properties
    /// <summary>
    /// Возвращает или устанавливает идентификатор.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Возвращает или устанавливает адрес.
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// Возвращает или устанавливает показания холодной воды в санузле.
    /// </summary>
    public int ColdWaterBathroom { get; set; }

    /// <summary>
    /// Возвращает или устанавливает показания холодной воды на кухне.
    /// </summary>
    public int? ColdWaterKitchen { get; set; }

    /// <summary>
    /// Возвращает или устанавливает полное имя.
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Возвращает или устанавливает показания горячей воды в санузле.
    /// </summary>
    public int HotWaterBathroom { get; set; }

    /// <summary>
    /// Возвращает или устанавливает показания горячей воды на кухне.
    /// </summary>
    public int? HotWaterKitchen { get; set; }

    /// <summary>
    /// Возвращает или устанавливает персональный номер клиента.
    /// </summary>
    public string PersonalNumber { get; set; }
    #endregion
}
