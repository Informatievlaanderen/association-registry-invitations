using System.Net;
using AssociationRegistry.Invitations.Api.Tests.Autofixture;
using AssociationRegistry.Invitations.Api.Tests.Fixture;
using AssociationRegistry.Invitations.Api.Uitnodingen.Requests;
using Newtonsoft.Json.Linq;

namespace AssociationRegistry.Invitations.Api.Tests.BijHetRegistrerenVanEenUitnodiging;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenUitnodigingZonderUitnodiger : IDisposable
{
    private readonly UitnodigingenApiClient _client;
    private readonly UitnodigingenApiFixture _fixture;
    private readonly UitnodigingsRequest _request;

    public GegevenEenUitnodigingZonderUitnodiger(UitnodigingenApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
        _request = new UitnodigingenFixture()
            .Build<UitnodigingsRequest>()
            .Without(u => u.Uitnodiger)
            .Create();
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.RegistreerUitnodiging(_request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        var response = await _client.RegistreerUitnodiging(_request);

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("Uitnodiger")
            .WhoseValue
            .Should().ContainEquivalentOf("Uitnodiger is verplicht.");
    }

    public void Dispose()
    {
        _fixture.ResetDatabase();
    }
}
