namespace AssociationRegistry.Invitations.Api.Tests.BijHetIntrekken.VanEenAanvraag;

using Fixture;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenReedsVerwerkteAanvraag
{
    private readonly TestApiFixture _fixture;
    private readonly TestApiClient _client;

    public GegevenEenReedsVerwerkteAanvraag(TestApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        foreach (var aanvraagId in _fixture.VerwerkteAanvraagIds)
        {
            var response = await _client.Aanvragen.TrekAanvraagIn(aanvraagId, _client);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        foreach (var aanvraagId in _fixture.VerwerkteAanvraagIds)
        {
            var response = await _client.Aanvragen.TrekAanvraagIn(aanvraagId, _client);

            var content = await response.Content.ReadAsStringAsync();
            var token = JToken.Parse(content);
            token["errors"]!.ToObject<Dictionary<string, string[]>>()
                .Should().ContainKey("aanvraag")
                .WhoseValue
                .Should().ContainEquivalentOf(Resources.IntrekkenAanvraagOnmogelijk);
        }
    }
}
