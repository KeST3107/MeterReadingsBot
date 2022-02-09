using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MeterReadingsBot
{
    using System;

    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel();
                    webBuilder.UseUrls("http://0.0.0.0:" + Environment.GetEnvironmentVariable("PORT"));
                });

    }
}
