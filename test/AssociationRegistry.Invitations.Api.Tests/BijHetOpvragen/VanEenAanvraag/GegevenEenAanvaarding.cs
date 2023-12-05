﻿namespace AssociationRegistry.Invitations.Api.Tests.BijHetOpvragen.VanEenAanvraag;

using Aanvragen.Registreer;
using Aanvragen.StatusWijziging;
using Infrastructure.Extensions;
using Autofixture;
using Fixture;
using Fixture.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenAanvaarding : IClassFixture<GegevenEenAanvaarding.Setup>
{
    private readonly TestApiClient _client;
    private readonly Setup _setup;

    public GegevenEenAanvaarding(TestApiFixture fixture, Setup setup)
    {
        _setup = setup;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse200()
    {
        var response = await _client.Aanvragen.GetAanvraagDetail(_setup.Aanvraag.Aanvrager.Insz, _setup.AanvraagId);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DanBevatDeBodyDeGeregistreerdeAanvraag()
    {
        var response = await _client.Aanvragen.GetAanvraagDetail(_setup.Aanvraag.Aanvrager.Insz, _setup.AanvraagId);
        var content = await response.Content.ReadAsStringAsync();

        var aanvraag = JsonConvert.DeserializeObject<JObject>(content,
                                                              new JsonSerializerSettings { DateParseHandling = DateParseHandling.None })!;

        aanvraag["aanvraagId"]!.Value<string>().Should().Be(_setup.AanvraagId.ToString());
        aanvraag["vCode"]!.Value<string>().Should().Be(_setup.Aanvraag.VCode);
        aanvraag["boodschap"]!.Value<string>().Should().Be(_setup.Aanvraag.Boodschap);
        aanvraag["status"]!.Value<string>().Should().Be(AanvraagStatus.Aanvaard.Status);

        aanvraag["datumRegistratie"]!.Value<string>().Should()
                                     .Be(_setup.AanvraagGeregistreerdOp.AsFormattedString());

        aanvraag["datumLaatsteAanpassing"]!.Value<string>().Should()
                                           .Be(_setup.AanvraagAanvaardOp.AsFormattedString());
        aanvraag["validator"]["vertegenwoordigerId"].Value<int>().Should().Be(_setup.VertegenwoordigerId);

        aanvraag["aanvrager"]!["insz"]!.Value<string>().Should().Be(_setup.Aanvraag.Aanvrager.Insz);
        aanvraag["aanvrager"]!["achternaam"]!.Value<string>().Should().Be(_setup.Aanvraag.Aanvrager.Achternaam);
        aanvraag["aanvrager"]!["voornaam"]!.Value<string>().Should().Be(_setup.Aanvraag.Aanvrager.Voornaam);
        aanvraag["aanvrager"]!["e-mail"]!.Value<string>().Should().Be(_setup.Aanvraag.Aanvrager.Email);
    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        public AanvraagRequest Aanvraag { get; set; }
        public Guid AanvraagId { get; set; }
        public Instant AanvraagGeregistreerdOp { get; set; }
        public Instant AanvraagAanvaardOp { get; set; }
        public int VertegenwoordigerId { get; set; }
        private readonly TestApiClient _client;
        private TestApiFixture _fixture;

        public Setup(TestApiFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.Clients.Authenticated;

            Aanvraag = new AutoFixture.Fixture().CustomizeAll()
                                                .Create<AanvraagRequest>();

            VertegenwoordigerId = new AutoFixture.Fixture().Create<int>();
        }

        public void Dispose()
        {
            _fixture.ResetDatabase();
        }

        public async Task InitializeAsync()
        {
            var response = await _client.Aanvragen.RegistreerAanvraag(Aanvraag)
                                        .EnsureSuccessOrThrowForAanvraag();

            AanvraagId = await response.ParseIdFromAanvraagResponse();

            AanvraagGeregistreerdOp = _fixture.Clock.PreviousInstant;

            await _client.Aanvragen.AanvaardAanvraag(
                AanvraagId, new WijzigAanvraagStatusRequest
                {
                    Validator = new Validator
                        { VertegenwoordigerId = VertegenwoordigerId },
                });

            AanvraagAanvaardOp = _fixture.Clock.PreviousInstant;
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
