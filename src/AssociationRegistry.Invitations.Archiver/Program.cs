using AssociationRegistry.Invitations.Api;
using AssociationRegistry.Invitations.Api.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
Console.WriteLine(env);

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        var postgreSqlOptionsSection = context.Configuration.GetPostgreSqlOptionsSection();
        var appSettings = context.Configuration.Get<AppSettings>();

        services
            .AddSingleton(appSettings)
            .AddSingleton(postgreSqlOptionsSection)
            .AddMarten(postgreSqlOptionsSection)
            .AddHostedService<ArchivalHostedService>();
    })
    .Build();

await host.RunAsync();