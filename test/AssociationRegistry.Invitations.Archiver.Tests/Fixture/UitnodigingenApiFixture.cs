using AssociationRegistry.Invitations.Api;
using AssociationRegistry.Invitations.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Invitations.Api.Infrastructure.Extensions;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using AssociationRegistry.Invitations.Archiver.Tests.Autofixture;
using AssociationRegistry.Invitations.Archiver.Tests.Fixture.Helpers;
using AssociationRegistry.Invitations.Archiver.Tests.Fixture.Stubs;
using AutoFixture;
using Marten;
using Marten.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using Npgsql;

namespace AssociationRegistry.Invitations.Archiver.Tests.Fixture;

public class UitnodigingenApiFixture : IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    private readonly IFixture? _autoFixture;
    public IHost Application { get; }
    public UitnodigingTestDataFactory TestDataFactory { get; }
    public ClockWithHistory Clock { get; }

    public UitnodigingenApiFixture()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.testrunner.json").Build();

        var postgreSqlOptionsSection = config.GetPostgreSqlOptionsSection();
        
        _autoFixture = new AutoFixture.Fixture()
            .CustomizeAll();
        
        WaitFor.Postgres.ToBecomeAvailable(new NullLogger<UitnodigingenApiFixture>(),
            GetConnectionString(postgreSqlOptionsSection, RootDatabase));

        DropCreateDatabase(postgreSqlOptionsSection);

        Clock = new ClockWithHistory();

        TestDataFactory = new UitnodigingTestDataFactory(SystemClock.Instance.GetCurrentInstant());

        Application = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(builder => builder.AddJsonFile("appsettings.testrunner.json").Build())
            .ConfigureServices((context, services) =>
            {
                var postgreSqlOptionsSection = context.Configuration.GetPostgreSqlOptionsSection();
                var appSettings = context.Configuration.Get<AppSettings>();

                services
                    .AddSingleton(appSettings)
                    .AddSingleton<Archival>()
                    .AddSingleton<IClock>(Clock)
                    .AddSingleton(postgreSqlOptionsSection)
                    .AddMarten(postgreSqlOptionsSection)
                    .InitializeMartenWith(new UitnodigingTestData(TestDataFactory))
                    .AddHostedService<Archival>();
            })
            .Build();
    }

    public async Task InitializeAsync()
    {
        await Application.Services.GetRequiredService<Archival>().StartAsync(CancellationToken.None);
    }

    public async Task DisposeAsync()
    {
        var store = Application.Services.GetRequiredService<IDocumentStore>();
        var session = store.LightweightSession();
        session.DeleteWhere<Uitnodiging>(u => true);
        await session.SaveChangesAsync();
    }

    private static void DropCreateDatabase(PostgreSqlOptionsSection postgreSqlOptionsSection)
    {
        using var connection = new NpgsqlConnection(GetConnectionString(postgreSqlOptionsSection, RootDatabase));
        using var cmd = connection.CreateCommand();

        try
        {
            connection.Open();
            // Ensure connections to DB are killed - there seems to be a lingering idle session after AssertDatabaseMatchesConfiguration(), even after store disposal
            cmd.CommandText +=
                $"DROP DATABASE IF EXISTS \"{postgreSqlOptionsSection.Database}\" WITH (FORCE);";
            cmd.CommandText +=
                $"CREATE DATABASE {postgreSqlOptionsSection.Database} WITH OWNER = {postgreSqlOptionsSection.Username};";
            cmd.ExecuteNonQuery();
        }
        catch (PostgresException ex)
        {
            if (ex.MessageText != $"database \"{postgreSqlOptionsSection.Database.ToLower()}\" already exists")
                throw;
        }
        finally
        {
            connection.Close();
            connection.Dispose();
        }
    }

    private static string GetConnectionString(PostgreSqlOptionsSection postgreSqlOptions, string? database = null)
        => $"host={postgreSqlOptions.Host};" +
           $"database={database ?? postgreSqlOptions.Database};" +
           $"password={postgreSqlOptions.Password};" +
           $"username={postgreSqlOptions.Username}";
}

public class UitnodigingTestData : IInitialData
{
    private readonly UitnodigingTestDataFactory _factory;
    public Dictionary<UitnodigingsStatus, IReadOnlyCollection<Uitnodiging>> Collection = new();
    
    public UitnodigingTestData(UitnodigingTestDataFactory testDataFactory)
    {
        _factory = testDataFactory;
    }
    
    public async Task Populate(IDocumentStore store, CancellationToken cancellation)
    {
        await using var session = store.LightweightSession();
        session.StoreObjects(_factory.Build());
        await session.SaveChangesAsync(cancellation);
    }
}

public class UitnodigingTestDataFactory
{
    private readonly IFixture? _autoFixture;
    public Uitnodigingen NietOverTijdUitnodigingen { get; }
    public Uitnodigingen OverTijdUitnodigingen { get; }
    public Instant Date { get; }
    
    
    public UitnodigingTestDataFactory(Instant date)
    {
        _autoFixture = new AutoFixture.Fixture()
            .CustomizeAll();
        
        Date = date;

        var wachtendOpAntwoord = _autoFixture.Create<Uitnodiging>();
        wachtendOpAntwoord.Status = UitnodigingsStatus.WachtOpAntwoord;
        wachtendOpAntwoord.DatumLaatsteAanpassing = date.AsFormattedString();

        NietOverTijdUitnodigingen = new Uitnodigingen(wachtendOpAntwoord);
        OverTijdUitnodigingen = new Uitnodigingen(wachtendOpAntwoord);
    }
    

    public IEnumerable<Uitnodiging> Build()
    {
        return new []{NietOverTijdUitnodigingen.WachtOpAntwoord, OverTijdUitnodigingen.WachtOpAntwoord};
    }
    
}
public record Uitnodigingen(Uitnodiging WachtOpAntwoord);
