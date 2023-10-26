using AssociationRegistry.Invitations.Hosts.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using Serilog.Debugging;

namespace AssociationRegistry.Invitations.Archiver;

using Microsoft.Extensions.Logging;
using System.Diagnostics;

public static class Program
{
    public static async Task Main(string[] args)
    {
        SelfLog.Enable(Console.WriteLine);

        var host = Host.CreateDefaultBuilder()
                       .ConfigureLogging(configure =>
                        {
                            configure.AddConsole();
                        })
                       .ConfigureServices(ConfigureDefaultServices)
                       .Build();

        var sw = Stopwatch.StartNew();
        await host.StartAsync();
        sw.Stop();

        var logger = host.Services.GetRequiredService<ILogger<ArchiverService>>();
        logger.LogInformation($"Het archiveren van uitnodigingen werd voltooid in {sw.ElapsedMilliseconds} ms.");
    }

    public static void ConfigureDefaultServices(HostBuilderContext context, IServiceCollection services)
    {
        var postgreSqlOptionsSection = context.Configuration.GetPostgreSqlOptionsSection();
        var archiverOptions = context.Configuration.Get<AppSettings>();

        services
           .AddSingleton<AppSettings>(archiverOptions)
           .AddSingleton(postgreSqlOptionsSection)
           .AddMarten(postgreSqlOptionsSection)
           .AddSingleton<IClock>(SystemClock.Instance)
           .AddHostedService<ArchiverService>();
    }
}