﻿namespace AssociationRegistry.Invitations.Api.Tests.BijHetIntrekken.VanEenUitnodiging;

using Fixture;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenReedsVerwerkteUitnodiging
{
    private readonly TestApiFixture _fixture;
    private readonly TestApiClient _client;

    public GegevenEenReedsVerwerkteUitnodiging(TestApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        foreach (var uitnodigingId in _fixture.VerwerkteUitnodigingsIds)
        {
            var response = await _client.Uitnodiging.TrekUitnodigingIn(uitnodigingId);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        foreach (var uitnodigingId in _fixture.VerwerkteUitnodigingsIds)
        {
            var response = await _client.Uitnodiging.TrekUitnodigingIn(uitnodigingId);

            var content = await response.Content.ReadAsStringAsync();
            var token = JToken.Parse(content);
            token["errors"]!.ToObject<Dictionary<string, string[]>>()
                .Should().ContainKey("uitnodiging")
                .WhoseValue
                .Should().ContainEquivalentOf(Resources.IntrekkenUitnodigingOnmogelijk);
        }
    }
}
