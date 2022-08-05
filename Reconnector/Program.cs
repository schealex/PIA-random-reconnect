using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reconnector.Options;

namespace Reconnector
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            var servicesProvider = BuildDi(config);

            using (servicesProvider as IDisposable)
            {
                var reconnector = servicesProvider.GetRequiredService<Reconnector>();
                var task = reconnector.Reconnect();
                task.Wait();
            }

            Environment.Exit(1);
        }

        private static IServiceProvider BuildDi(IConfiguration config)
        {
            return new ServiceCollection()
                .AddSingleton<PiaCtl>()
                .AddSingleton<Reconnector>()
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.SetMinimumLevel(LogLevel.Information);
                    loggingBuilder.AddConsole();
                })
                // Add options
                .AddOptions()
                .Configure<PiaOptions>(config.GetSection("PIA"))
                // Finalize
                .BuildServiceProvider();
        }
    }
}