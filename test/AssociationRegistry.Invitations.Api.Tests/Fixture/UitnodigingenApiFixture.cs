using AssociationRegistry.Invitations.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Invitations.Api.Infrastructure.Extensions;
using AssociationRegistry.Invitations.Api.Tests.Autofixture;
using AssociationRegistry.Invitations.Api.Tests.Fixture.Helpers;
using AssociationRegistry.Invitations.Api.Tests.Fixture.Stubs;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Requests;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;
using NodaTime;
using Npgsql;

namespace AssociationRegistry.Invitations.Api.Tests.Fixture;

public class UitnodigingenApiFixture : IAsyncLifetime
{
    private const string RootDatabase = @"postgres";

    private readonly WebApplicationFactory<Program> _application;
    private readonly IFixture? _autoFixture;

    public Clients Clients { get; }
    public ClockWithHistory Clock { get; }
    public List<Guid> VerwerkteUitnodigingIds { get; } = new();

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

        _application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(
                builder =>
                {
                    builder.UseContentRoot(Directory.GetCurrentDirectory());
                    builder.ConfigureTestServices(services => services.AddSingleton<IClock>(Clock));
                });

        Clients = new Clients(
            config.GetSection(nameof(OAuth2IntrospectionOptions))
                .Get<OAuth2IntrospectionOptions>()!,
            _application.CreateClient);
    }

    public async Task InitializeAsync()
    {
        await CreateReedsVerwerkteUitnodiging(id => Clients.Authenticated.AanvaardUitnodiging(id));
        await CreateReedsVerwerkteUitnodiging(id => Clients.Authenticated.WeigerUitnodiging(id));
        await CreateReedsVerwerkteUitnodiging(id => Clients.Authenticated.TrekUitnodigingIn(id));
    }

    public async Task DisposeAsync()
    {
        var store = _application.Services.GetRequiredService<IDocumentStore>();
        var session = store.LightweightSession();
        session.DeleteWhere<Uitnodiging>(u => true);
        await session.SaveChangesAsync();
    }

    private async Task CreateReedsVerwerkteUitnodiging(Func<Guid, Task> verwerkActie)
    {
        var uitnodiging = _autoFixture.Create<UitnodigingsRequest>();
        uitnodiging.Uitgenodigde.Insz = _autoFixture.Create<TestInsz>();
            
        var response = await Clients.Authenticated.RegistreerUitnodiging(uitnodiging).EnsureSuccessOrThrow();
        var content = await response.Content.ReadAsStringAsync();
            
        var uitnodigingId = Guid.Parse(JToken.Parse(content)["uitnodigingId"]!.Value<string>()!);
        VerwerkteUitnodigingIds.Add(uitnodigingId);

        await verwerkActie(uitnodigingId);
    }
    
    public void ResetDatabase()
    {
        // var store = _application.Services.GetRequiredService<IDocumentStore>();
        // var session = store.LightweightSession();
        // session.DeleteWhere<Uitnodiging>(u => true);
        // session.SaveChanges();
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