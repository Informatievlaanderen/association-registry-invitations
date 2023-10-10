using System.Net;
using AssociationRegistry.Invitations.Api.Tests.Fixture;
using Newtonsoft.Json.Linq;

namespace AssociationRegistry.Invitations.Api.Tests.BijHetIntrekkenVanEenUitnodiging;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenOnbekendeUitnodiging : IDisposable
{
    private readonly UitnodigingenApiFixture _fixture;
    private readonly UitnodigingenApiClient _client;

    public GegevenEenOnbekendeUitnodiging(UitnodigingenApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.TrekUitnodigingIn(Guid.NewGuid());
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        var response = await _client.TrekUitnodigingIn(Guid.NewGuid());

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("uitnodiging")
            .WhoseValue
            .Should().ContainEquivalentOf("Deze uitnodiging is niet gekend.");
    }

    public void Dispose()
    {
        _fixture.ResetDatabase();
    }
}
