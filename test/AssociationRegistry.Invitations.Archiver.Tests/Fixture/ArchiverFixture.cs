namespace AssociationRegistry.Invitations.Archiver.Tests.Fixture;

using Aanvragen;
using Api.Infrastructure.ConfigurationBindings;
using Autofixture;
using AutoFixture;
using Helpers;
using Hosts.Infrastructure.Extensions;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using NodaTime;
using Npgsql;
using Stubs;
using Uitnodigingen;

public class ArchiverFixture : IAsyncLifetime
{
    private const string RootDatabase = @"postgres";
    private readonly IFixture? _autoFixture;
    public IHost Application { get; }
    public UitnodigingTestDataFactory UitnodigingTestDataFactory { get; }
    public AanvraagTestDataFactory AanvraagTestDataFactory { get; }
    public ClockWithHistory Clock { get; }

    public ArchiverFixture()
    {
        var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.testrunner.json").Build();

        var postgreSqlOptionsSection = config.GetPostgreSqlOptionsSection();
        var archiverOptions = config.Get<AppSettings>();

        _autoFixture = new Fixture()
           .CustomizeAll();

        WaitFor.Postgres.ToBecomeAvailable(new NullLogger<ArchiverFixture>(),
                                           GetConnectionString(postgreSqlOptionsSection, RootDatabase));

        DropCreateDatabase(postgreSqlOptionsSection);

        Clock = new ClockWithHistory();

        UitnodigingTestDataFactory = new UitnodigingTestDataFactory(SystemClock.Instance.GetCurrentInstant(), archiverOptions);
        AanvraagTestDataFactory = new AanvraagTestDataFactory(SystemClock.Instance.GetCurrentInstant(), archiverOptions);

        Application = Host.CreateDefaultBuilder()
                          .ConfigureAppConfiguration(builder => builder.AddJsonFile("appsettings.testrunner.json").Build())
                          .ConfigureServices((context, services) =>
                           {
                               Program.ConfigureServices(context, services);

                               services
                                  .AddSingleton<IClock>(Clock)
                                  .InitializeMartenWith(
                                       new UitnodigingTestData(UitnodigingTestDataFactory),
                                       new AanvraagTestData(AanvraagTestDataFactory)
                                  );
                           })
                          .ConfigureLogging((context, builder) =>
                           {
                               Program.ConfigureLogger(context, builder);
                           })
                          .Build();
    }

    public async Task InitializeAsync()
    {
        await Application.StartAsync(CancellationToken.None);
    }

    public async Task DisposeAsync()
    {
        var store = Application.Services.GetRequiredService<IDocumentStore>();
        var session = store.LightweightSession();
        session.DeleteWhere<Uitnodiging>(u => true);
        session.DeleteWhere<Aanvraag>(u => true);
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