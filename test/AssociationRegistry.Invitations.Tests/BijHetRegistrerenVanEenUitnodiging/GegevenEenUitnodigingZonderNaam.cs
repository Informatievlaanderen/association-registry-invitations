﻿using System.Net;
using AssociationRegistry.Invitations.Tests.Autofixture;
using AssociationRegistry.Invitations.Tests.Fixture;
using AssociationRegistry.Invitations.Uitnodingen.Requests;
using Newtonsoft.Json.Linq;

namespace AssociationRegistry.Invitations.Tests.BijHetRegistrerenVanEenUitnodiging;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenUitnodigingZonderNaam : IDisposable
{
    private readonly UitnodigingenApiClient _client;
    private readonly UitnodigingenApiFixture _fixture;
    private readonly UitnodigingsRequest _request;

    public GegevenEenUitnodigingZonderNaam(UitnodigingenApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
        _request = new UitnodigingenFixture()
            .Build<UitnodigingsRequest>()
            .With(u => u.Uitgenodigde,
                new UitnodigingenFixture()
                    .Build<Uitgenodigde>()
                    .Without(u2=>u2.Naam)
                    .Create())
            .Create();
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.RegistreerUitnodiging(_request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        var response = await _client.RegistreerUitnodiging(_request);

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("Uitgenodigde.Naam")
            .WhoseValue
            .Should().ContainEquivalentOf("Naam is verplicht.");
    }

    public void Dispose()
    {
        _fixture.RestDatabase();
    }
}