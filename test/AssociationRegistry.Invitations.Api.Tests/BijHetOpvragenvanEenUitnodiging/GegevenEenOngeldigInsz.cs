﻿using System.Net;
using AssociationRegistry.Invitations.Api.Tests.Autofixture;
using AssociationRegistry.Invitations.Api.Tests.Fixture;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Requests;
using Newtonsoft.Json.Linq;
using NodaTime;

namespace AssociationRegistry.Invitations.Api.Tests.BijHetOpvragenvanEenUitnodiging;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenOngeldigInsz : IClassFixture<GegevenEenOngeldigInsz.Setup>
{
    private readonly Setup _setup;
    private readonly UitnodigingenApiClient _client;

    public GegevenEenOngeldigInsz(UitnodigingenApiFixture fixture, Setup setup)
    {
        _setup = setup;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.GetUitnodigingsDetail("99.99.99-999.64", _setup.UitnodigingsId);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DanBevatDeBodyEenError()
    {
        var response = await _client.GetUitnodigingsDetail("99.99.99-999.64", _setup.UitnodigingsId);
        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("insz")
            .WhoseValue
            .Should().ContainEquivalentOf("Deze uitnodiging is niet voor deze persoon bestemd.");
    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        public UitnodigingsRequest Uitnodiging { get; set; }
        public Guid UitnodigingsId { get; set; }
        public Instant UitnodigingAangemaaktOp { get; set; }

        private readonly UitnodigingenApiClient _client;
        private UitnodigingenApiFixture _fixture;

        public Setup(UitnodigingenApiFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.Clients.Authenticated;

            Uitnodiging = new AutoFixture.Fixture().CustomizeAll()
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
            UitnodigingAangemaaktOp = _fixture.Clock.PreviousInstant;
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
