using MeterReadingsBot.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeterReadingsBot.Dal;

/// <summary>
/// Прдставляет контекст приложения.
/// </summary>
public class BotContext : DbContext
{
    #region .ctor
    /// <summary>
    /// Инициализирует новый экземпляр типа <see cref="BotContext" />
    /// </summary>
    /// <param name="options">Настройки.</param>
    public BotContext(DbContextOptions<BotContext> options) : base(options)
    {
    }
    #endregion

    #region Properties
    /// <summary>
    /// Возвращает или устанавливает сущности клиентов с состояниями диалога.
    /// </summary>
    public DbSet<StartUserClient> StartUserClients { get; private set; }

    /// <summary>
    /// Возвращает или устанавливает сущности временных клиентов.
    /// </summary>
    public DbSet<Client> TempClients { get; private set; }

    /// <summary>
    /// Возвращает или устанавливает базовые сущности клиентов.
    /// </summary>
    public DbSet<UserClientBase> UserClients { get; private set; }

    /// <summary>
    /// Возвращает или устанавливает сущности клиентов передачи показаний.
    /// </summary>
    public DbSet<WaterReadingsUserClient> WaterReadingsUserClients { get; private set; }

    /// <summary>
    /// Возвращает или устанавливает сущности суперадминов.
    /// </summary>
    public DbSet<AdminUserClient> AdminUserClients { get; private set; }
    #endregion

    #region Overrided
    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserClientBase>()
            .HasDiscriminator<string>("DISCRIMINATOR")
            .HasValue<WaterReadingsUserClient>(nameof(WaterReadingsUserClient))
            .HasValue<AdminUserClient>(nameof(AdminUserClient))
            .HasValue<StartUserClient>(nameof(StartUserClient));
        modelBuilder.Entity<UserClientBase>()
            .HasKey(clientBase => clientBase.Id);
        modelBuilder.Entity<Client>()
            .HasKey(client => client.Id);
    }
    #endregion
}
