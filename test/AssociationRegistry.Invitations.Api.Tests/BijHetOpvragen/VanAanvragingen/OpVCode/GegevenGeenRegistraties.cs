namespace AssociationRegistry.Invitations.Api.Tests.BijHetOpvragen.VanAanvragingen.OpVCode;

using Fixture;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenGeenRegistraties
{
    private readonly UitnodigingenApiClient _client;

    public GegevenGeenRegistraties(UitnodigingenApiFixture fixture)
    {
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse200()
    {
        var response = await _client.GetAanvragingenOpVcode("V0000001");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DanBevatDeBodyGeenAanvragingen()
    {
        var response = await _client.GetAanvragingenOpVcode("V0000001");
        var content = await response.Content.ReadAsStringAsync();

        var token = JToken.Parse(content);
        token["aanvragingen"].Should()
            .BeEmpty();
    }
}
