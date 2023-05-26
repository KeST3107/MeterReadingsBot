using MeterReadingsBot.Entities;

namespace MeterReadingsBot.Repositories;

/// <summary>
/// Представляет репозиторий клиентов передачи показаний.
/// </summary>
public interface IWaterReadingsClientRepository : IUserClientRepository<WaterReadingsUserClient>
{
}
