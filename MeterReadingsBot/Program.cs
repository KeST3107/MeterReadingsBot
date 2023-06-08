using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MeterReadingsBot;

public static class Program
{
    #region Public
    /// <summary>
    /// Создает хост.
    /// </summary>
    /// <param name="args">Аргументы.</param>
    /// <returns>Хост</returns>
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                /*webBuilder.UseKestrel();
                webBuilder.UseUrls("http://0.0.0.0:" + Environment.GetEnvironmentVariable("PORT"));*/
            });
    }
    /// <summary>
    /// Определяет точку входа приложения.
    /// </summary>
    /// <param name="args">Аргументы.</param>
    public static void Main(string[] args)
    {
        CreateHostBuilder(args)
            .Build()
            .Run();
    }
    #endregion
}
