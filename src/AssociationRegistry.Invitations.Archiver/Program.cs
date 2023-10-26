using AssociationRegistry.Invitations.Hosts.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using Serilog.Debugging;

namespace AssociationRegistry.Invitations.Archiver;

using Destructurama;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Diagnostics;

public static class Program
{
    public static async Task Main(string[] args)
    {
        SelfLog.Enable(Console.WriteLine);

        var host = Host.CreateDefaultBuilder()
                       .ConfigureAppConfiguration(builder =>
                                                      builder.AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json",
                                                                          optional: true,
                                                                          reloadOnChange: false))
                       .ConfigureServices(ConfigureServices)
                       .ConfigureLogging(ConfigureLogger)
                       .Build();

        var sw = Stopwatch.StartNew();
        await host.StartAsync();
        sw.Stop();

        var logger = host.Services.GetRequiredService<ILogger<ArchiverService>>();
        logger.LogInformation($"Het archiveren van uitnodigingen werd voltooid in {sw.ElapsedMilliseconds} ms.");
    }

    public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
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

    public static void ConfigureLogger(HostBuilderContext context, ILoggingBuilder builder)
    {
        var loggerConfig =
            new LoggerConfiguration()
               .ReadFrom.Configuration(context.Configuration)
               .Enrich.FromLogContext()
               .Enrich.WithMachineName()
               .Enrich.WithThreadId()
               .Enrich.WithEnvironmentUserName()
               .Destructure.JsonNetTypes();

        var logger = loggerConfig.CreateLogger();

        Log.Logger = logger;

        builder
            //.AddSerilog(logger)
           .AddOpenTelemetry();
    }
}