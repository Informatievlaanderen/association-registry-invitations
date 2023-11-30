namespace AssociationRegistry.Invitations.Api.Tests.BijHetWijgeren.VanEenUitnodiging;

using Fixture;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenReedsVerwerkteUitnodiging
{
    private readonly UitnodigingenApiFixture _fixture;
    private readonly UitnodigingenApiClient _client;

    public GegevenEenReedsVerwerkteUitnodiging(UitnodigingenApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        foreach (var id in _fixture.VerwerkteUitnodigingsIds)
        {
            var response = await _client.WijgerAanvraag(id);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }


    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        foreach (var id in _fixture.VerwerkteUitnodigingsIds)
        {
            var response = await _client.WijgerAanvraag(id);

            var content = await response.Content.ReadAsStringAsync();
            var token = JToken.Parse(content);
            token["errors"]!.ToObject<Dictionary<string, string[]>>()
                .Should().ContainKey("aanvraag")
                .WhoseValue
                .Should().ContainEquivalentOf(Resources.WeigerenAanvraagOnmogelijk);
        }
    }
}
