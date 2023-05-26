using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MeterReadingsBot;

public static class Program
{
    #region Public
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
    public static void Main(string[] args)
    {
        CreateHostBuilder(args)
            .Build()
            .Run();
    }
    #endregion
}
