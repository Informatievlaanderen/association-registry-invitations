namespace AssociationRegistry.Invitations.Api.Tests.BijHetAanvaarden.VanEenAanvraag;

using Fixture;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenOnbekendeAanvraag
{
    private readonly TestApiFixture _fixture;
    private readonly TestApiClient _client;

    public GegevenEenOnbekendeAanvraag(TestApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.Aanvragen.AanvaardAanvraag(Guid.NewGuid(), _client);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        var response = await _client.Aanvragen.AanvaardAanvraag(Guid.NewGuid(), _client);

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("aanvraag")
            .WhoseValue
            .Should().ContainEquivalentOf("Deze aanvraag is niet gekend.");
    }

    public void Dispose()
    {
        _fixture.ResetDatabase();
    }
}
