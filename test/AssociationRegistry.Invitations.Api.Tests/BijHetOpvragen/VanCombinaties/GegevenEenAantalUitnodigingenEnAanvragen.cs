namespace AssociationRegistry.Invitations.Api.Tests.BijHetOpvragen.VanCombinaties;

using Aanvragen.StatusWijziging;
using Aanvragen.Registreer;
using Infrastructure.Extensions;
using Autofixture;
using Fixture;
using Fixture.Extensions;
using Uitnodigingen.Registreer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime;
using System.Net;
using Uitnodigingen.StatusWijziging;
using Fixture = AutoFixture.Fixture;

[Collection(TestApiCollection.Name)]
public class GegevenEenAantalUitnodigingenEnAanvragen : IClassFixture<GegevenEenAantalUitnodigingenEnAanvragen.Setup>
{
    private readonly TestApiClient _client;
    private readonly Setup _setup;

    public GegevenEenAantalUitnodigingenEnAanvragen(TestApiFixture fixture, Setup setup)
    {
        _setup = setup;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse200()
    {
        var response = await _client.GetGecombineerdResultaat(_setup.VCode);

        response.StatusCode.Should().Be(HttpStatusCode.OK, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task DanBevatDeBodyDeGeregistreerdeAanvraag()
    {
        var response = await _client.GetGecombineerdResultaat(_setup.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var token = JsonConvert.DeserializeObject<JObject>(content,
                                                           new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });

        foreach (var (aanvraagId, aanvraagRequest, tijdstip, vertegenwoordigerId) in _setup.Aanvragen)
        {
            var aanvraag = token!["aanvragen"].Should()
                                              .ContainSingle(u => u["aanvraagId"]!.Value<string>() == aanvraagId.ToString()).Subject;

            aanvraag["vCode"]!.Value<string>().Should().Be(_setup.VCode);
            aanvraag["boodschap"]!.Value<string>().Should().Be(aanvraagRequest.Boodschap);
            aanvraag["status"]!.Value<string>().Should().Be(AanvraagStatus.Aanvaard.Status);

            aanvraag["datumRegistratie"]!.Value<string>().Should()
                                         .Be(tijdstip.AsFormattedString());

            aanvraag["validator"]["vertegenwoordigerId"].Value<int>().Should().Be(vertegenwoordigerId);

            aanvraag["aanvrager"]!["insz"]!.Value<string>().Should().Be(aanvraagRequest.Aanvrager.Insz);
            aanvraag["aanvrager"]!["achternaam"]!.Value<string>().Should().Be(aanvraagRequest.Aanvrager.Achternaam);
            aanvraag["aanvrager"]!["voornaam"]!.Value<string>().Should().Be(aanvraagRequest.Aanvrager.Voornaam);
            aanvraag["aanvrager"]!["e-mail"]!.Value<string>().Should().Be(aanvraagRequest.Aanvrager.Email);
        }

        foreach (var (uitnodigingId, uitnodigingRequest, tijdstip, vertegenwoordigerId) in _setup.Uitnodigingen)
        {
            var uitnodiging = token!["uitnodigingen"].Should()
                                                     .ContainSingle(u => u["uitnodigingId"]!.Value<string>() == uitnodigingId.ToString())
                                                     .Subject;

            uitnodiging["vCode"]!.Value<string>().Should().Be(_setup.VCode);
            uitnodiging["boodschap"]!.Value<string>().Should().Be(uitnodigingRequest.Boodschap);
            uitnodiging["status"]!.Value<string>().Should().Be(UitnodigingsStatus.Geweigerd.Status);

            uitnodiging["datumRegistratie"]!.Value<string>().Should()
                                            .Be(tijdstip.AsFormattedString());

            uitnodiging["uitnodiger"]!["vertegenwoordigerId"]!.Value<int>().Should()
                                                              .Be(uitnodigingRequest.Uitnodiger.VertegenwoordigerId);

            uitnodiging["uitgenodigde"]!["insz"]!.Value<string>().Should().Be(uitnodigingRequest.Uitgenodigde.Insz);
            uitnodiging["uitgenodigde"]!["achternaam"]!.Value<string>().Should().Be(uitnodigingRequest.Uitgenodigde.Achternaam);
            uitnodiging["uitgenodigde"]!["voornaam"]!.Value<string>().Should().Be(uitnodigingRequest.Uitgenodigde.Voornaam);
            uitnodiging["uitgenodigde"]!["e-mail"]!.Value<string>().Should().Be(uitnodigingRequest.Uitgenodigde.Email);
        }
    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        public (Guid, AanvraagRequest, Instant, int)[] Aanvragen { get; set; } = { };
        public (Guid, UitnodigingsRequest, Instant, int)[] Uitnodigingen { get; set; } = { };
        public string VCode { get; set; }
        private readonly TestApiClient _client;
        private TestApiFixture _fixture;

        public Setup(TestApiFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.Clients.Authenticated;
        }

        public void Dispose()
        {
            _fixture.ResetDatabase();
        }

        public async Task InitializeAsync()
        {
            var autoFixture = new AutoFixture.Fixture().CustomizeAll();

            VCode = autoFixture.Create<AanvraagRequest>().VCode;

            foreach (var request in autoFixture.CreateMany<AanvraagRequest>())
            {
                request.VCode = VCode;

                var response = await _client.Aanvragen.RegistreerAanvraag(request)
                                            .EnsureSuccessOrThrowForAanvraag();

                var aanvraagId = await response.ParseIdFromAanvraagResponse();
                var vertegenwoordigerId = new Fixture().Create<int>();

                await _client.Aanvragen.AanvaardAanvraag(aanvraagId,
                                                         new WijzigAanvraagStatusRequest
                                                         {
                                                             Validator = new Aanvragen.StatusWijziging.Validator
                                                                 { VertegenwoordigerId = vertegenwoordigerId },
                                                         });

                var aanvraagAanvaardOp = _fixture.Clock.PreviousInstant;
                Aanvragen = Aanvragen.Append((aanvraagId, request, aanvraagAanvaardOp, vertegenwoordigerId)).ToArray();
            }

            foreach (var request in autoFixture.CreateMany<UitnodigingsRequest>())
            {
                request.VCode = VCode;

                var response = await _client.Uitnodiging.RegistreerUitnodiging(request)
                                            .EnsureSuccessOrThrowForUitnodiging();

                var uitnodigingId = await response.ParseIdFromUitnodigingResponse();
                var vertegenwoordigerId = new Fixture().Create<int>();

                await _client.Uitnodiging.WeigerUitnodiging(uitnodigingId);

                var uitnodigingAanvaardOp = _fixture.Clock.PreviousInstant;
                Uitnodigingen = Uitnodigingen.Append((uitnodigingId, request, uitnodigingAanvaardOp, vertegenwoordigerId)).ToArray();
            }
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
