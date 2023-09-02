using System.Collections.Generic;
using MeterReadingsBot.Entities;

namespace MeterReadingsBot.Repositories;

/// <summary>
/// Представляет репозиторий суперадминов.
/// </summary>
public interface IAdminUserRepository : IUserClientRepository<AdminUserClient>
{
    /// <summary>
    /// Возвращает коллекцию <see cref="AdminUserClient"/>.
    /// </summary>
    /// <returns>Возвращает коллекцию клиентов.</returns>
    IReadOnlyCollection<AdminUserClient> GetAll();

    /// <summary>
    /// Удаляет клиента <see cref="AdminUserClient"/>.
    /// </summary>
    void Remove(AdminUserClient client);
}
