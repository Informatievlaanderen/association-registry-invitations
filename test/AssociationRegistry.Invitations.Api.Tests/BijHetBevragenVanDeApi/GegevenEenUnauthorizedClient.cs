using System.Net;
using AssociationRegistry.Invitations.Api.Tests.Fixture;

namespace AssociationRegistry.Invitations.Api.Tests.BijHetBevragenVanDeApi;

[Collection(TestApiCollection.Name)]
public class GivenAnUnauthorizedClient
{
    private readonly TestApiClient _client;

    public GivenAnUnauthorizedClient(TestApiFixture fixture)
    {
        _client = fixture.Clients.Unauthorized;
    }


    [Fact]
    public async Task Then_It_Returns_403_With_Unauthorized_Client()
    {
        var response = await _client.Uitnodiging.GetUitnodigingsDetail("", Guid.NewGuid(), _client);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task Then_It_Returns_200_For_Health()
    {
        var response = await _client.GetHealth();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
