using AssociationRegistry.Invitations.Tests.Fixture.Helpers;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace AssociationRegistry.Invitations.Tests.Fixture;

public class UitnodigingenApiFixture
{
    private readonly WebApplicationFactory<Program> _application;
    public Clients Clients { get; }

    public UitnodigingenApiFixture()
    {
        _application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(
                builder =>
                {
                    builder.UseContentRoot(Directory.GetCurrentDirectory());
                });
        Clients = new Clients(
            GetConfiguration().GetSection(nameof(OAuth2IntrospectionOptions))
                .Get<OAuth2IntrospectionOptions>(),
            _application.CreateClient);
    }

    private IConfigurationRoot GetConfiguration()
    {
        var tempConfiguration = ConfigurationHelper.GetConfiguration();

        return tempConfiguration;
    }
}
