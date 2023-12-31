﻿namespace AssociationRegistry.Invitations.Api.Tests.BijHetOpvragen.VanEenUitnodiging;

using Autofixture;
using Fixture;
using Fixture.Extensions;
using Uitnodigingen.Registreer;
using Newtonsoft.Json.Linq;
using NodaTime;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenOngeldigInsz : IClassFixture<GegevenEenOngeldigInsz.Setup>
{
    private readonly Setup _setup;
    private readonly TestApiClient _client;

    public GegevenEenOngeldigInsz(TestApiFixture fixture, Setup setup)
    {
        _setup = setup;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.Uitnodiging.GetUitnodigingsDetail("99.99.99-999.64", _setup.UitnodigingId);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DanBevatDeBodyEenError()
    {
        var response = await _client.Uitnodiging.GetUitnodigingsDetail("99.99.99-999.64", _setup.UitnodigingId);
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
        public Guid UitnodigingId { get; set; }
        public Instant UitnodigingGeregistreerdOp { get; set; }

        private readonly TestApiClient _client;
        private TestApiFixture _fixture;

        public Setup(TestApiFixture fixture)
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
            var response = await _client.Uitnodiging.RegistreerUitnodiging(Uitnodiging)
                                        .EnsureSuccessOrThrowForUitnodiging();
            
            UitnodigingId = await response.ParseIdFromUitnodigingResponse();
            UitnodigingGeregistreerdOp = _fixture.Clock.PreviousInstant;
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
