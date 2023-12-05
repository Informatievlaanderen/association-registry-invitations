namespace AssociationRegistry.Invitations.Api.Tests.BijHetWeigeren.VanEenAanvraag;

using Aanvragen.StatusWijziging;
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
        foreach (var id in _fixture.VerwerkteAanvraagIds)
        {
            var response =
                await _client.Aanvragen.WeigerAanvraag(
                    id, new WijzigAanvraagStatusRequest
                        { Validator = new Validator
                            { VertegenwoordigerId = 1 } });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        foreach (var id in _fixture.VerwerkteAanvraagIds)
        {
            var response =
                await _client.Aanvragen.WeigerAanvraag(
                    id, new WijzigAanvraagStatusRequest
                        { Validator = new Validator
                            { VertegenwoordigerId = 1 } });

            var content = await response.Content.ReadAsStringAsync();
            var token = JToken.Parse(content);

            token["errors"]!.ToObject<Dictionary<string, string[]>>()
                            .Should().ContainKey("aanvraag")
                            .WhoseValue
                            .Should().ContainEquivalentOf(Resources.WeigerenAanvraagOnmogelijk);
        }
    }
}
