using AssociationRegistry.Invitations.Hosts.Infrastructure.Extensions;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Debugging;

namespace AssociationRegistry.Invitations.Archiver;

public static class Program
{
    public static async Task Main(string[] args)
    {
        SelfLog.Enable(Console.WriteLine);

        
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(ConfigureDefaultServices)
            .Build();

        await host.RunAsync();
    }

    public static void ConfigureDefaultServices(HostBuilderContext context, IServiceCollection services) {
        var postgreSqlOptionsSection = context.Configuration.GetPostgreSqlOptionsSection();
        var archiverOptions = context.Configuration.Get<AppSettings>();

        services
            .AddSingleton<AppSettings>(archiverOptions)
            .AddSingleton(postgreSqlOptionsSection)
            .AddMarten(postgreSqlOptionsSection)
            .AddHostedService<ArchiverService>();
    }    
}