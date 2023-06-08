using System.Collections.Generic;
using MeterReadingsBot.Entities;

namespace MeterReadingsBot.Repositories;

/// <summary>
/// Представляет репозиторий базовых клиентов.
/// </summary>
public interface IUserClientRepository
{
    #region Overridable
    /// <summary>
    /// Добавляет базового клиента.
    /// </summary>
    /// <param name="userClient">Базовый клиент.</param>
    void Add(UserClientBase userClient);

    /// <summary>
    /// Выполняет поиск базового клиента по идентификатору чата.
    /// </summary>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <returns>Базовый клиент.</returns>
    UserClientBase FindBy(long chatId);

    /// <summary>
    /// Возвращает всех базовых клиентов.
    /// </summary>
    /// <returns>Коллекция базовых клиентов.</returns>
    IReadOnlyCollection<UserClientBase> GetAll();

    /// <summary>
    /// Удаляет базового клиента.
    /// </summary>
    /// <param name="client">Базовый клиент.</param>
    void Remove(UserClientBase client);

    /// <summary>
    /// Обновляет базового клиента.
    /// </summary>
    /// <param name="userClient">Базовый клиент.</param>
    void Update(UserClientBase userClient);
    #endregion
}
