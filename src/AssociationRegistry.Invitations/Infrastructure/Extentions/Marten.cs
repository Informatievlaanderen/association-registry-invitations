﻿using AssociationRegistry.Invitations.Infrastructure.ConfigurationBindings;
using JasperFx.CodeGeneration;
using Marten;
using Marten.Events;
using Marten.Services;
using Newtonsoft.Json;
using Weasel.Core;

namespace AssociationRegistry.Invitations.Infrastructure.Extentions;

public static class Marten
{
    public static IServiceCollection AddMarten(
        this IServiceCollection services,
        PostgreSqlOptionsSection postgreSqlOptions)
    {
        var martenConfiguration = services.AddMarten(
            serviceProvider =>
            {
                var opts = new StoreOptions();
                opts.Connection(postgreSqlOptions.GetConnectionString());
                opts.Events.StreamIdentity = StreamIdentity.AsString;
                opts.Serializer(CreateMartenSerializer());
                opts.Events.MetadataConfig.EnableAll();

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
            s => { s.DateParseHandling = DateParseHandling.None; });

        return jsonNetSerializer;
    }
}
