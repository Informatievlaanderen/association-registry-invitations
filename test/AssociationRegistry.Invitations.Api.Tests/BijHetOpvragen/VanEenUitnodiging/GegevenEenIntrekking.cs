﻿namespace AssociationRegistry.Invitations.Api.Tests.BijHetOpvragen.VanEenUitnodiging;

using Infrastructure.Extensions;
using Autofixture;
using Fixture;
using Fixture.Extensions;
using Uitnodigingen.Registreer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenIntreking : IClassFixture<GegevenEenIntreking.Setup>
{
    private readonly TestApiClient _client;
    private readonly Setup _setup;

    public GegevenEenIntreking(TestApiFixture fixture, Setup setup)
    {
        _setup = setup;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse200()
    {
        var response = await _client.Uitnodiging.GetUitnodigingsDetail(_setup.Uitnodiging.Uitgenodigde.Insz, _setup.UitnodigingId);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DanBevatDeBodyDeGeregistreerdeUitnodiging()
    {
        var response = await _client.Uitnodiging.GetUitnodigingsDetail(_setup.Uitnodiging.Uitgenodigde.Insz, _setup.UitnodigingId);
        var content = await response.Content.ReadAsStringAsync();

        var uitnodiging = JsonConvert.DeserializeObject<JObject>(content,
            new JsonSerializerSettings { DateParseHandling = DateParseHandling.None })!;
        uitnodiging["uitnodigingId"]!.Value<string>().Should().Be(_setup.UitnodigingId.ToString());
        uitnodiging["vCode"]!.Value<string>().Should().Be(_setup.Uitnodiging.VCode);
        uitnodiging["boodschap"]!.Value<string>().Should().Be(_setup.Uitnodiging.Boodschap);
        uitnodiging["status"]!.Value<string>().Should().Be(UitnodigingsStatus.Ingetrokken.Status);
        uitnodiging["datumRegistratie"]!.Value<string>().Should()
            .Be(_setup.UitnodigingGeregistreerdOp.AsFormattedString());
        uitnodiging["datumLaatsteAanpassing"]!.Value<string>().Should()
            .Be(_setup.UitnodigingIngetrokkenOp.AsFormattedString());
        uitnodiging["uitnodiger"]!["vertegenwoordigerId"]!.Value<int>().Should()
                                                          .Be(_setup.Uitnodiging.Uitnodiger.VertegenwoordigerId);
        uitnodiging["uitgenodigde"]!["insz"]!.Value<string>().Should().Be(_setup.Uitnodiging.Uitgenodigde.Insz);
        uitnodiging["uitgenodigde"]!["achternaam"]!.Value<string>().Should().Be(_setup.Uitnodiging.Uitgenodigde.Achternaam);
        uitnodiging["uitgenodigde"]!["voornaam"]!.Value<string>().Should().Be(_setup.Uitnodiging.Uitgenodigde.Voornaam);
        uitnodiging["uitgenodigde"]!["e-mail"]!.Value<string>().Should().Be(_setup.Uitnodiging.Uitgenodigde.Email);
    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        public UitnodigingsRequest Uitnodiging { get; set; }
        public Guid UitnodigingId { get; set; }

        public Instant UitnodigingGeregistreerdOp { get; set; }
        public Instant UitnodigingIngetrokkenOp { get; set; }

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

            await _client.Uitnodiging.TrekUitnodigingIn(UitnodigingId);

            UitnodigingIngetrokkenOp = _fixture.Clock.PreviousInstant;
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
