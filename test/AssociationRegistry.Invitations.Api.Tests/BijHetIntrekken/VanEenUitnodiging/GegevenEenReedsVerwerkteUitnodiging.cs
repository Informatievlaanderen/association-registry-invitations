namespace AssociationRegistry.Invitations.Api.Tests.BijHetIntrekken.VanEenUitnodiging;

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
        foreach (var uitnodigingId in _fixture.VerwerkteUitnodigingsIds)
        {
            var response = await _client.TrekUitnodigingIn(uitnodigingId);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        foreach (var uitnodigingId in _fixture.VerwerkteUitnodigingsIds)
        {
            var response = await _client.TrekUitnodigingIn(uitnodigingId);

            var content = await response.Content.ReadAsStringAsync();
            var token = JToken.Parse(content);
            token["errors"]!.ToObject<Dictionary<string, string[]>>()
                .Should().ContainKey("uitnodiging")
                .WhoseValue
                .Should().ContainEquivalentOf(Resources.IntrekkenUitnodigingOnmogelijk);
        }
    }
}
