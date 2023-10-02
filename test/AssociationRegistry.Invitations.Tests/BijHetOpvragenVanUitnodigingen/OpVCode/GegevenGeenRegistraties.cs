using System.Net;

namespace AssociationRegistry.Invitations.Tests.BijHetOpvragenVanUitnodigingen.OpVCode;

using Fixture;

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
        var response = await _client.GetUitnodigingenOpVcode("V0000001");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
