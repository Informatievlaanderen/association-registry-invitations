using System.Net;
using AssociationRegistry.Invitations.Tests.Fixture;

namespace AssociationRegistry.Invitations.Tests.WhenAccessingTheApi;

[Collection(UitnodigingenApiCollection.Name)]
public class GivenAnUnauthenticatedClient
{
    private readonly UitnodigingenApiClient _client;

    public GivenAnUnauthenticatedClient(UitnodigingenApiFixture fixture)
    {
        _client = fixture.Clients.Unauthenticated;
    }

    [Fact]
    public async Task Then_It_Returns_401_With_Unauthenticated_Client()
    {
        var response = await _client.GetRoot();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
