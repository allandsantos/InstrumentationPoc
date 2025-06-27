using InstrumentationPoc.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace InstrumentationPoc;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();
        
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) => services
                .AddSerilog()
                .AddLoggingInterceptor()
                .AddServices())
            .Build();
        
        var appService = host.Services.GetRequiredService<AppService>();
        await appService.RunAsync();
        
        Console.WriteLine("Demo completed. Press any key to exit...");
        Console.ReadKey();
    }
}