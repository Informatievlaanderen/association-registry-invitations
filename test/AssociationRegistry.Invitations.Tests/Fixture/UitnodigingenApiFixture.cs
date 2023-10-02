using AssociationRegistry.Invitations.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Invitations.Infrastructure.Extentions;
using AssociationRegistry.Invitations.Tests.Fixture.Helpers;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Npgsql;

namespace AssociationRegistry.Invitations.Tests.Fixture;

public class UitnodigingenApiFixture
{
    private const string RootDatabase = @"postgres";

    private readonly WebApplicationFactory<Program> _application;
    public Clients Clients { get; }

    public UitnodigingenApiFixture()
    {
        var postgreSqlOptionsSection = GetConfiguration().GetPostgreSqlOptionsSection();
        WaitFor.Postgres.ToBecomeAvailable(new NullLogger<UitnodigingenApiFixture>(),
            GetConnectionString(postgreSqlOptionsSection, RootDatabase));

        DropCreateDatabase(postgreSqlOptionsSection);

        _application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(
                builder => { builder.UseContentRoot(Directory.GetCurrentDirectory()); });
        Clients = new Clients(
            GetConfiguration().GetSection(nameof(OAuth2IntrospectionOptions))
                .Get<OAuth2IntrospectionOptions>()!,
            _application.CreateClient);
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

    private IConfigurationRoot GetConfiguration()
    {
        var tempConfiguration = ConfigurationHelper.GetConfiguration();

        return tempConfiguration;
    }

    private static string GetConnectionString(PostgreSqlOptionsSection postgreSqlOptions, string? database = null)
        => $"host={postgreSqlOptions.Host};" +
           $"database={database ?? postgreSqlOptions.Database};" +
           $"password={postgreSqlOptions.Password};" +
           $"username={postgreSqlOptions.Username}";
}
