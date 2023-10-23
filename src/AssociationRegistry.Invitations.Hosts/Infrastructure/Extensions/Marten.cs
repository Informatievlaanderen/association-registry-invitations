using AssociationRegistry.Invitations.Api.Infrastructure.ConfigurationBindings;
using JasperFx.CodeGeneration;
using Marten;
using Marten.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Weasel.Core;

namespace AssociationRegistry.Invitations.Hosts.Infrastructure.Extensions;

public static class Marten
{
    public static IServiceCollection AddMarten(
        this IServiceCollection services,
        PostgreSqlOptionsSection postgreSqlOptions)
    {
        services.AddSingleton(postgreSqlOptions);

        var martenConfiguration = services.AddMarten(
            serviceProvider =>
            {
                var opts = new StoreOptions();
                opts.Connection(postgreSqlOptions.GetConnectionString());
                opts.Serializer(CreateMartenSerializer());

                opts.RegisterDocumentType<Uitnodiging>();

                if (serviceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment())
                {
                    opts.GeneratedCodeMode = TypeLoadMode.Dynamic;
                }
                else
                {
                    opts.GeneratedCodeMode = TypeLoadMode.Auto;
                    opts.SourceCodeWritingEnabled = false;
                }

                opts.AutoCreateSchemaObjects = AutoCreate.All;
                return opts;
            });

        martenConfiguration.ApplyAllDatabaseChangesOnStartup();

        return services;
    }

    private static string GetConnectionString(this PostgreSqlOptionsSection postgreSqlOptions)
        => $"host={postgreSqlOptions.Host};" +
           $"database={postgreSqlOptions.Database};" +
           $"password={postgreSqlOptions.Password};" +
           $"username={postgreSqlOptions.Username}";

    private static JsonNetSerializer CreateMartenSerializer()
    {
        var jsonNetSerializer = new JsonNetSerializer();

        jsonNetSerializer.Customize(
            s =>
            {
                s.DateParseHandling = DateParseHandling.None;
            });


        return jsonNetSerializer;
    }
}
