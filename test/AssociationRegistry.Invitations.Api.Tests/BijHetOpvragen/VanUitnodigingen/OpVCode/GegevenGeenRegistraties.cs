﻿namespace AssociationRegistry.Invitations.Api.Tests.BijHetOpvragen.VanUitnodigingen.OpVCode;

using Fixture;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenGeenRegistraties
{
    private readonly TestApiClient _client;

    public GegevenGeenRegistraties(TestApiFixture fixture)
    {
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse200()
    {
        var response = await _client.Uitnodiging.GetUitnodigingenOpVcode("V0000001");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DanBevatDeBodyGeenUitnodigingen()
    {
        var response = await _client.Uitnodiging.GetUitnodigingenOpVcode("V0000001");
        var content = await response.Content.ReadAsStringAsync();

        var token = JToken.Parse(content);
        token["uitnodigingen"].Should()
            .BeEmpty();
    }
}
