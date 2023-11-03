using System.Collections.Generic;
using MeterReadingsBot.Entities;

namespace MeterReadingsBot.Repositories;

/// <summary>
/// Представляет репозиторий стартовых клиентов.
/// </summary>
public interface IStartUserClientRepository : IUserClientRepository<StartUserClient>
{
    /// <summary>
    /// Возвращает коллекцию <see cref="StartUserClient"/>.
    /// </summary>
    /// <returns>Возвращает коллекцию клиентов.</returns>
    IReadOnlyCollection<StartUserClient> GetAll();
}
