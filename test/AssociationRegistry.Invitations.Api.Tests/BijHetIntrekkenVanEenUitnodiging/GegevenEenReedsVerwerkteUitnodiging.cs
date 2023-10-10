﻿using System.Net;
using AssociationRegistry.Invitations.Api.Tests.Autofixture;
using AssociationRegistry.Invitations.Api.Tests.Fixture;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Requests;
using Newtonsoft.Json.Linq;

namespace AssociationRegistry.Invitations.Api.Tests.BijHetIntrekkenVanEenUitnodiging;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenReedsVerwerkteUitnodiging
{
    private readonly UitnodigingenApiFixture _fixture;
    private readonly UitnodigingenApiClient _client;

    public GegevenEenReedsVerwerkteUitnodiging(UitnodigingenApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
    }
    
    [Fact]
    public async Task DanIsDeResponse400()
    {
        foreach (var uitnodigingsId in _fixture.VerwerkteUitnodigingIds)
        {
            var response = await _client.TrekUitnodigingIn(uitnodigingsId);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        foreach (var uitnodigingsId in _fixture.VerwerkteUitnodigingIds)
        {
            var response = await _client.TrekUitnodigingIn(uitnodigingsId);

            var content = await response.Content.ReadAsStringAsync();
            var token = JToken.Parse(content);
            token["errors"]!.ToObject<Dictionary<string, string[]>>()
                .Should().ContainKey("uitnodiging")
                .WhoseValue
                .Should().ContainEquivalentOf(Resources.IntrekkenOnmogelijk);
        }
    }
}
