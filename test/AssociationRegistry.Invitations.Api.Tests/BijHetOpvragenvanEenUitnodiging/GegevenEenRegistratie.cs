using System.Globalization;
using System.Net;
using AssociationRegistry.Invitations.Api.Tests.Autofixture;
using AssociationRegistry.Invitations.Api.Tests.Fixture;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Requests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime;

namespace AssociationRegistry.Invitations.Api.Tests.BijHetOpvragenvanEenUitnodiging;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenRegistratie : IClassFixture<GegevenEenRegistratie.Setup>
{
    private readonly UitnodigingenApiClient _client;
    private readonly Setup _setup;

    public GegevenEenRegistratie(UitnodigingenApiFixture fixture, Setup setup)
    {
        _setup = setup;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse200()
    {
        var response = await _client.GetUitnodigingsDetail(_setup.Uitnodiging.Uitgenodigde.Insz, _setup.UitnodigingsId);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DanBevatDeBodyDeGeregistreerdeUitnodiging()
    {
        var response = await _client.GetUitnodigingsDetail(_setup.Uitnodiging.Uitgenodigde.Insz, _setup.UitnodigingsId);
        var content = await response.Content.ReadAsStringAsync();

        var uitnodiging = JsonConvert.DeserializeObject<JObject>(content,
            new JsonSerializerSettings { DateParseHandling = DateParseHandling.None })!;
        uitnodiging["id"]!.Value<string>().Should().Be(_setup.UitnodigingsId.ToString());
        uitnodiging["vCode"]!.Value<string>().Should().Be(_setup.Uitnodiging.VCode);
        uitnodiging["boodschap"]!.Value<string>().Should().Be(_setup.Uitnodiging.Boodschap);
        uitnodiging["status"]!.Value<string>().Should().Be(UitnodigingsStatus.WachtOpAntwoord.Status);
        uitnodiging["datumRegistratie"]!.Value<string>().Should()
            .Be(_setup.UitnodigingAangemaaktOp.ToString("g", CultureInfo.InvariantCulture));
        uitnodiging["uitnodiger"]!["vertegenwoordigerId"]!.Value<int>().Should()
            .Be(_setup.Uitnodiging.Uitnodiger.VertegenwoordigerId);
        uitnodiging["uitgenodigde"]!["insz"]!.Value<string>().Should().Be(_setup.Uitnodiging.Uitgenodigde.Insz);
        uitnodiging["uitgenodigde"]!["naam"]!.Value<string>().Should().Be(_setup.Uitnodiging.Uitgenodigde.Naam);
        uitnodiging["uitgenodigde"]!["voornaam"]!.Value<string>().Should().Be(_setup.Uitnodiging.Uitgenodigde.Voornaam);
        uitnodiging["uitgenodigde"]!["email"]!.Value<string>().Should().Be(_setup.Uitnodiging.Uitgenodigde.Email);
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
