using System;

namespace MeterReadingsBot.Models;

/// <summary>
/// Модель данных клиента.
/// </summary>
/// <param name="Address">Адрес.</param>
/// <param name="FullName">Полное имя.</param>
/// <param name="PersonnelNumber">Персональный номер.</param>
/// <param name="LastReadingsDateTime">Дата последнего показания.</param>
public record ClientDto(string Address, string FullName, string PersonnelNumber, DateTime? LastReadingsDateTime)
{
}
