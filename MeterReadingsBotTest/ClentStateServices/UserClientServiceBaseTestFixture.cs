using System;
using MeterReadingsBot.Dal;
using MeterReadingsBot.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MeterReadingsBotTest.ClentStateServices;

[TestFixture]
public abstract class UserClientServiceBaseTestFixture
{
    #region Data
    #region Fields
    protected IConfiguration Configuration = null!;
    #endregion
    #endregion

    #region Setup/Teardown
    [SetUp]
    protected virtual void SetUp()
    {

    }
    #endregion

    #region Properties
    protected IServiceProvider ServiceProvider { get; private set; } = null!;
    #endregion

    #region Protected
    protected virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddSettings(Configuration);
        services.AddServices();
        services.AddDbContext<BotContext>
            (optionsBuilder => optionsBuilder.UseInMemoryDatabase(nameof(BotContext) + "_Test"));
        services.AddHttpClient("tgwebhook")
            .AddTypedClient<ITelegramBotClient>(httpClient
                => CreateMockTelegramBot());
    }

    protected TService GetService<TService>() where TService : notnull
    {
        var scope = ServiceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<TService>();
    }

    [OneTimeSetUp]
    protected void OneTimeSetup()
    {
        Configuration = InitConfiguration();
        var collection = new ServiceCollection();
        ConfigureServices(collection);
        ServiceProvider = collection.BuildServiceProvider();
    }
    #endregion

    #region Private
    private static IConfiguration InitConfiguration()
    {
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json")
            .AddEnvironmentVariables();
        var configuration = builder.Build();
        return configuration;
    }

    private TelegramBotClient CreateMockTelegramBot()
    {
        var mockBot = new Mock<TelegramBotClient>();
        return mockBot.Object;
    }
    #endregion
}
