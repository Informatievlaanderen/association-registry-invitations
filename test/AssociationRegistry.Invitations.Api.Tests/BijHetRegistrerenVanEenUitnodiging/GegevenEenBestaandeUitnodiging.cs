﻿using System.Net;
using AssociationRegistry.Invitations.Api.Tests.Autofixture;
using AssociationRegistry.Invitations.Api.Tests.Fixture;
using AssociationRegistry.Invitations.Api.Uitnodingen.Requests;
using Newtonsoft.Json.Linq;

namespace AssociationRegistry.Invitations.Api.Tests.BijHetRegistrerenVanEenUitnodiging;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenBestaandeUitnodiging : IClassFixture<GegevenEenBestaandeUitnodiging.Setup>
{
    private readonly UitnodigingenApiClient _client;
    private readonly UitnodigingsRequest _request;

    public GegevenEenBestaandeUitnodiging(UitnodigingenApiFixture fixture, Setup setup)
    {
        _client = fixture.Clients.Authenticated;
        _request = new AutoFixture.Fixture()
            .Customize(new GeldigeUitnodigingen(setup.Uitnodiging.VCode, setup.Uitnodiging.Uitgenodigde.Insz))
            .Create<UitnodigingsRequest>();
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
            .Should().ContainKey("Uitnodiging")
            .WhoseValue
            .Should().ContainEquivalentOf("Deze vertegenwoordiger is reeds uitgenodigd.");
    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        public UitnodigingsRequest Uitnodiging { get; set; }
        public Guid UitnodigingsId { get; set; }

        private readonly UitnodigingenApiClient _client;
        private UitnodigingenApiFixture _fixture;

        public Setup(UitnodigingenApiFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.Clients.Authenticated;

            Uitnodiging = new AutoFixture.Fixture()
                .Customize(new GeldigeUitnodigingen())
                .Create<UitnodigingsRequest>();
        }

        public void Dispose()
        {
            _fixture.ResetDatabase();
        }

        public async Task InitializeAsync()
        {
            var response = await _client.RegistreerUitnodiging(Uitnodiging);
            var content = await response.Content.ReadAsStringAsync();
            UitnodigingsId = Guid.Parse(JToken.Parse(content)["id"]!.Value<string>()!);
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}