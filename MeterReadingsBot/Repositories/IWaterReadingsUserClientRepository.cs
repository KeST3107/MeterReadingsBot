using System.Collections.Generic;
using MeterReadingsBot.Entities;

namespace MeterReadingsBot.Repositories;

/// <summary>
/// Представляет репозиторий клиентов передачи показаний.
/// </summary>
public interface IWaterReadingsClientRepository : IUserClientRepository<WaterReadingsUserClient>
{
    /// <summary>
    /// Возвращает коллекцию клиентов передачи показаний.
    /// </summary>
    /// <returns>Коллекция клиентов передачи показаний</returns>
    IReadOnlyCollection<WaterReadingsUserClient> GetAll();
}
