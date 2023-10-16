using System.Net;
using AssociationRegistry.Invitations.Api.Tests.Fixture;

namespace AssociationRegistry.Invitations.Api.Tests.BijHetBevragenVanDeApi;

[Collection(UitnodigingenApiCollection.Name)]
public class GivenAnUnauthorizedClient
{
    private readonly UitnodigingenApiClient _client;

    public GivenAnUnauthorizedClient(UitnodigingenApiFixture fixture)
    {
        _client = fixture.Clients.Unauthorized;
    }


    [Fact]
    public async Task Then_It_Returns_403_With_Unauthorized_Client()
    {
        var response = await _client.GetUitnodigingsDetail("", Guid.NewGuid());
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task Then_It_Returns_200_For_Health()
    {
        var response = await _client.GetHealth();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
