using System.Net;
using AssociationRegistry.Invitations.Tests.Fixture;
using AssociationRegistry.Invitations.Uitnodingen.Requests;

namespace AssociationRegistry.Invitations.Tests.BijHetRegistrerenVanEenUitnodiging;


[Collection(UitnodigingenApiCollection.Name)]
public class HappyPath
{
    private readonly UitnodigingenApiClient _client;

    public HappyPath(UitnodigingenApiFixture fixture)
    {
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse201()
    {
        var response = await _client.RegistreerUitnodiging(new UitnodigingsRequest
        {
            
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}