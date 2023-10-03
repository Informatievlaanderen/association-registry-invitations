﻿using System.Net;
using AssociationRegistry.Invitations.Api.Tests.Fixture;

namespace AssociationRegistry.Invitations.Api.Tests.BijHetBevragenVanDeApi;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenUnauthenticatedClient
{
    private readonly UitnodigingenApiClient _client;

    public GegevenEenUnauthenticatedClient(UitnodigingenApiFixture fixture)
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