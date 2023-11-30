namespace AssociationRegistry.Invitations.Api.Tests.BijHetIntrekken.VanEenAanvraag;

using Fixture;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenReedsVerwerkteAanvraag
{
    private readonly UitnodigingenApiFixture _fixture;
    private readonly UitnodigingenApiClient _client;

    public GegevenEenReedsVerwerkteAanvraag(UitnodigingenApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        foreach (var uitnodigingId in _fixture.VerwerkteAanvraagIds)
        {
            var response = await _client.TrekAanvraagIn(uitnodigingId);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        foreach (var uitnodigingId in _fixture.VerwerkteAanvraagIds)
        {
            var response = await _client.TrekAanvraagIn(uitnodigingId);

            var content = await response.Content.ReadAsStringAsync();
            var token = JToken.Parse(content);
            token["errors"]!.ToObject<Dictionary<string, string[]>>()
                .Should().ContainKey("aanvraag")
                .WhoseValue
                .Should().ContainEquivalentOf(Resources.IntrekkenAanvraagOnmogelijk);
        }
    }
}
