using System.Net;
using AssociationRegistry.Invitations.Tests.Fixture;

namespace AssociationRegistry.Invitations.Tests.WhenAccessingTheApi;

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
        var response = await _client.GetRoot();
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
