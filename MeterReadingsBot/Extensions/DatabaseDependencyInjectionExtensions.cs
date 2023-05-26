using System;
using MeterReadingsBot.Dal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MeterReadingsBot.Extensions;

/// <summary>
/// Определяет расширения <see cref="IServiceCollection"/> для базы данных.
/// </summary>
public static class DatabaseDependencyInjectionExtensions
{
    #region Public

    /// <summary>
    /// Добавляет зависимости для работы с БД.
    /// </summary>
    /// <param name="services">Экземпляр <see cref="IServiceCollection"/></param>
    /// <param name="configuration">Экземпляр <see cref="IConfiguration"/></param>
    /// <exception cref="ArgumentNullException">Если один из аргументов не задан.</exception>
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        services.AddDbContext<BotContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSqlConnection")));

    }
    #endregion
}
