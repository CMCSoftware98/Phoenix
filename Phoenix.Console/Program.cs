using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Phoenix.Console.Services;
using System;

namespace Phoenix
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<PhoenixStatsCollector>();
                })
                .RunConsoleAsync();
        }
    }
}