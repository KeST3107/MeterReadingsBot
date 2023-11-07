using System;
using System.Collections.Generic;
using System.Linq;
using MeterReadingsBot.Dal;
using MeterReadingsBot.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeterReadingsBot.Repositories;

/// <summary>
/// Определяет репозиторий клиентов.
/// </summary>
public class UserClientRepository : IUserClientRepository,
    IWaterReadingsClientRepository,
    IStartUserClientRepository,
    IAdminUserRepository
{
    #region Data
    #region Fields
    private readonly BotContext _context;
    #endregion
    #endregion

    #region .ctor
    /// <summary>
    /// Инициализирует новый экземпляр типа <see cref="UserClientRepository" />
    /// </summary>
    /// <param name="context">Контекст доступа к данным.</param>
    /// <exception cref="ArgumentNullException">Если <see cref="BotContext"/> не задан.</exception>
    public UserClientRepository(BotContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));

    }
    #endregion

    #region IUserClientRepository members
    /// <inheritdoc />
    public void Add(UserClientBase userClient)
    {
        _context.UserClients.Add(userClient);
        _context.SaveChanges();
    }

    /// <inheritdoc />
    public AdminUserClient Add(AdminUserClient userClient)
    {
        _context.AdminUserClients.Add(userClient);
        _context.SaveChanges();
        return userClient;
    }

    AdminUserClient IUserClientRepository<AdminUserClient>.FindBy(long chatId)
    {
        return _context.AdminUserClients.SingleOrDefault(clientBase => clientBase.ChatId == chatId);
    }

    /// <inheritdoc />
    public void Update(AdminUserClient userClient)
    {
        _context.AdminUserClients.Update(userClient);
        _context.SaveChanges();
    }

    IReadOnlyCollection<AdminUserClient> IAdminUserRepository.GetAll()
    {
        return _context.AdminUserClients.ToList();
    }

    /// <inheritdoc />
    public IReadOnlyCollection<UserClientBase> GetAllBy(long chatId)
    {
       return _context.UserClients.Where(client => client.ChatId == chatId).ToList();
    }

    /// <inheritdoc />
    public void Remove(AdminUserClient client)
    {
        _context.AdminUserClients.Remove(client);
        _context.SaveChanges();
    }

    /// <inheritdoc />
    public UserClientBase FindBy(long chatId)
    {
        return _context.UserClients.SingleOrDefault(clientBase => clientBase.ChatId == chatId);
    }

    /// <inheritdoc />
    public IReadOnlyCollection<UserClientBase> GetAll()
    {
        return _context.UserClients.ToList();
    }

    /// <inheritdoc />
    public void Remove(UserClientBase client)
    {
        _context.UserClients.Remove(client);
        _context.SaveChanges();
    }

    /// <inheritdoc />
    public void Update(UserClientBase userClient)
    {
        _context.UserClients.Update(userClient);
        _context.SaveChanges();
    }
    #endregion

    #region IUserClientRepository<StartUserClient> members
    /// <inheritdoc />
    public StartUserClient Add(StartUserClient userClient)
    {
        _context.StartUserClients.Add(userClient);
        _context.SaveChanges();
        return userClient;
    }
    StartUserClient IUserClientRepository<StartUserClient>.FindBy(long chatId)
    {
        return _context.StartUserClients.SingleOrDefault(startUserClient => startUserClient.ChatId == chatId);
    }

    /// <inheritdoc />
    public void Update(StartUserClient userClient)
    {
        _context.StartUserClients.Update(userClient);
        _context.SaveChanges();
    }

    IReadOnlyCollection<StartUserClient> IStartUserClientRepository.GetAll()
    {
        return _context.StartUserClients.ToList();
    }
    #endregion

    #region IUserClientRepository<WaterReadingsUserClient> members
    /// <inheritdoc />
    public WaterReadingsUserClient Add(WaterReadingsUserClient userClient)
    {
        _context.WaterReadingsUserClients.Add(userClient);
        _context.SaveChanges();
        return userClient;
    }
    WaterReadingsUserClient IUserClientRepository<WaterReadingsUserClient>.FindBy(long chatId)
    {
        return _context.WaterReadingsUserClients.Include(waterReadingsUserClient => waterReadingsUserClient.TempClient)
            .SingleOrDefault(waterReadingsUserClient => waterReadingsUserClient.ChatId == chatId);
    }

    /// <inheritdoc />
    public void Update(WaterReadingsUserClient userClient)
    {
        _context.WaterReadingsUserClients.Update(userClient);
        _context.SaveChanges();
    }

    IReadOnlyCollection<WaterReadingsUserClient> IWaterReadingsClientRepository.GetAll()
    {
        return _context.WaterReadingsUserClients.ToList();
    }
    #endregion
}
