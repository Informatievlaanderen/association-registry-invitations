namespace AssociationRegistry.Invitations.Api.Tests.BijHetOpvragen.VanEenAanvraag;

using Aanvragen.Registreer;
using Infrastructure.Extensions;
using Autofixture;
using Fixture;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime;
using System.Net;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenIntreking : IClassFixture<GegevenEenIntreking.Setup>
{
    private readonly UitnodigingenApiClient _client;
    private readonly Setup _setup;

    public GegevenEenIntreking(UitnodigingenApiFixture fixture, Setup setup)
    {
        _setup = setup;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse200()
    {
        var response = await _client.GetAanvraagDetail(_setup.Aanvraag.Aanvrager.Insz, _setup.AanvraagId);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DanBevatDeBodyDeGeregistreerdeUitnodiging()
    {
        var response = await _client.GetAanvraagDetail(_setup.Aanvraag.Aanvrager.Insz, _setup.AanvraagId);
        var content = await response.Content.ReadAsStringAsync();

        var aanvraag = JsonConvert.DeserializeObject<JObject>(content,
            new JsonSerializerSettings { DateParseHandling = DateParseHandling.None })!;
        aanvraag["aanvraagId"]!.Value<string>().Should().Be(_setup.AanvraagId.ToString());
        aanvraag["vCode"]!.Value<string>().Should().Be(_setup.Aanvraag.VCode);
        aanvraag["boodschap"]!.Value<string>().Should().Be(_setup.Aanvraag.Boodschap);
        aanvraag["status"]!.Value<string>().Should().Be(UitnodigingsStatus.Ingetrokken.Status);
        aanvraag["datumRegistratie"]!.Value<string>().Should()
            .Be(_setup.AanvraagGeregistreerdOp.AsFormattedString());
        aanvraag["datumLaatsteAanpassing"]!.Value<string>().Should()
            .Be(_setup.AanvraagIngetrokkenOp.AsFormattedString());
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
        public Instant AanvraagIngetrokkenOp { get; set; }

        private readonly UitnodigingenApiClient _client;
        private UitnodigingenApiFixture _fixture;

        public Setup(UitnodigingenApiFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.Clients.Authenticated;

            Aanvraag = new AutoFixture.Fixture().CustomizeAll()
                .Create<AanvraagRequest>();
        }

        public void Dispose()
        {
            _fixture.ResetDatabase();
        }

        public async Task InitializeAsync()
        {
            var response = await _client.RegistreerAanvraag(Aanvraag)
                .EnsureSuccessOrThrowForAanvraag();

            AanvraagId = await response.ParseIdFromAanvraagResponse();
            AanvraagGeregistreerdOp = _fixture.Clock.PreviousInstant;

            await _client.TrekAanvraagIn(AanvraagId);

            AanvraagIngetrokkenOp = _fixture.Clock.PreviousInstant;
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
