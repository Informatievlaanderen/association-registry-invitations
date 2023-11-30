﻿namespace AssociationRegistry.Invitations.Api.Tests.BijHetAanvaarden.VanEenAanvraag;

using Fixture;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenOnbekendeAanvraag
{
    private readonly UitnodigingenApiFixture _fixture;
    private readonly UitnodigingenApiClient _client;

    public GegevenEenOnbekendeAanvraag(UitnodigingenApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.AanvaardAanvraag(Guid.NewGuid());
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        var response = await _client.AanvaardAanvraag(Guid.NewGuid());

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("aanvraag")
            .WhoseValue
            .Should().ContainEquivalentOf("Deze aanvraging is niet gekend.");
    }

    public void Dispose()
    {
        _fixture.ResetDatabase();
    }
}
