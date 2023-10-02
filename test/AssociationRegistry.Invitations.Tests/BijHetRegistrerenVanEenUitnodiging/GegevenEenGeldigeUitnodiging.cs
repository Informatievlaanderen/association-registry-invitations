using System.Net;
using AssociationRegistry.Invitations.Tests.Fixture;
using AssociationRegistry.Invitations.Uitnodingen.Requests;
using Newtonsoft.Json.Linq;

namespace AssociationRegistry.Invitations.Tests.BijHetRegistrerenVanEenUitnodiging;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenGeldigeUitnodiging : IDisposable
{
    private readonly UitnodigingenApiClient _client;
    private UitnodigingenApiFixture _fixture;

    public GegevenEenGeldigeUitnodiging(UitnodigingenApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse201()
    {
        var response = await _client.RegistreerUitnodiging(new UitnodigingsRequest
        {
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task DanHeeftInDeBodyEenGuid()
    {
        var response = await _client.RegistreerUitnodiging(new UitnodigingsRequest
        {
        });

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        Guid.TryParse(token["id"]!.Value<string>(), out _).Should().BeTrue();
    }

    public void Dispose()
    {
        _fixture.RestDatabase();
    }
}
