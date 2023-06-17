using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using shg.dynRunner.Application.Interfaces;
using shg.dynRunner.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace DynamicRun
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args)
                .RunConsoleAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseConsoleLifetime()
                .ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Warning))
                .ConfigureServices((hostContext, services) =>
                {
                    //services.Configure<MyServiceOptions>(hostContext.Configuration);
                    services.AddTransient<IGetDynCodes, ExampleGetDynCodes>();
                    services.AddHostedService<DynRunnerHostedService>();
                    services.AddSingleton(Console.Out);
                });
    }
}
