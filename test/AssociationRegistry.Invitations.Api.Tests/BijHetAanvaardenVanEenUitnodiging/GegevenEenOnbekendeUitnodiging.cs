using System.Net;
using AssociationRegistry.Invitations.Api.Tests.Fixture;

namespace AssociationRegistry.Invitations.Api.Tests.BijHetAanvaardenVanEenUitnodiging;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenOnbekendeUitnodiging
{
    private readonly UitnodigingenApiFixture _fixture;
    private readonly UitnodigingenApiClient _client;

    public GegevenEenOnbekendeUitnodiging(UitnodigingenApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse404()
    {
        var response = await _client.AanvaardUitnodiging(Guid.NewGuid());
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public void Dispose()
    {
        _fixture.ResetDatabase();
    }
}
