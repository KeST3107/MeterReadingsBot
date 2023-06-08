using MeterReadingsBot.Entities;

namespace MeterReadingsBot.Repositories;

/// <summary>
/// Представляет базовый репозиторий клиентов.
/// </summary>
/// <typeparam name="TUserClient">Тип базового клиента.</typeparam>
public interface IUserClientRepository<TUserClient> where TUserClient : UserClientBase
{
    #region Overridable
    /// <summary>
    /// Добавляет клиента с типом <see cref="TUserClient"/>.
    /// </summary>
    /// <param name="userClient">Клиент типа <see cref="TUserClient"/>.</param>
    /// <returns>Возвращает клиента с типом <see cref="TUserClient"/>.</returns>
    TUserClient Add(TUserClient userClient);

    /// <summary>
    /// Выполняет поиск клиента <see cref="TUserClient"/> по идентификатору чата.
    /// </summary>
    /// <param name="chatId">Идентификатор чата.</param>
    /// <returns>Возвращает клиента с типом <see cref="TUserClient"/>.</returns>
    TUserClient FindBy(long chatId);

    /// <summary>
    /// Обновляет клиента с типом <see cref="TUserClient"/>.
    /// </summary>
    /// <param name="userClient">Клиент типа <see cref="TUserClient"/>.</param>
    void Update(TUserClient userClient);
    #endregion
}
