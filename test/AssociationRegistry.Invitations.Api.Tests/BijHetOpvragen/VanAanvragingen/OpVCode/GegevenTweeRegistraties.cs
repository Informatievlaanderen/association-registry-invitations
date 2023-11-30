namespace AssociationRegistry.Invitations.Api.Tests.BijHetOpvragen.VanAanvragingen.OpVCode;

using Aanvragen.Registreer;
using Infrastructure.Extensions;
using Autofixture;
using Fixture;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime;
using System.Net;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenTweeRegistraties : IClassFixture<GegevenTweeRegistraties.Setup>
{
    private readonly UitnodigingenApiClient _client;
    private readonly Setup _setup;

    public GegevenTweeRegistraties(UitnodigingenApiFixture fixture, Setup setup)
    {
        _setup = setup;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse200()
    {
        var response = await _client.GetAanvragingenOpVcode("V0000001");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DanBevatDeBodyDeGeregistreerdeAanvragingen()
    {
        var response = await _client.GetAanvragingenOpVcode(_setup.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var token = JsonConvert.DeserializeObject<JObject>(content,
            new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });

        var aanvraag1 = token!["aanvragingen"].Should()
            .ContainSingle(u => u["aanvraagid"]!.Value<string>() == _setup.AanvraagId1.ToString()).Subject;
        aanvraag1["vCode"]!.Value<string>().Should().Be(_setup.VCode);
        aanvraag1["boodschap"]!.Value<string>().Should().Be(_setup.Aanvraag1.Boodschap);
        aanvraag1["status"]!.Value<string>().Should().Be(UitnodigingsStatus.WachtOpAntwoord.Status);
        aanvraag1["datumRegistratie"]!.Value<string>().Should()
            .Be(_setup.Aanvraag1AangemaaktOp.AsFormattedString());
        aanvraag1["aanvrager"]!["insz"]!.Value<string>().Should().Be(_setup.Aanvraag1.Aanvrager.Insz);
        aanvraag1["aanvrager"]!["achternaam"]!.Value<string>().Should().Be(_setup.Aanvraag1.Aanvrager.Achternaam);
        aanvraag1["aanvrager"]!["voornaam"]!.Value<string>().Should().Be(_setup.Aanvraag1.Aanvrager.Voornaam);
        aanvraag1["aanvrager"]!["e-mail"]!.Value<string>().Should().Be(_setup.Aanvraag1.Aanvrager.Email);

        var aanvraag2 = token["uitnodigingen"].Should()
            .ContainSingle(u => u["uitnodigingId"]!.Value<string>() == _setup.AanvraagId2.ToString()).Subject;
        aanvraag2["vCode"]!.Value<string>().Should().Be(_setup.VCode);
        aanvraag2["boodschap"]!.Value<string>().Should().Be(_setup.Aanvraag2.Boodschap);
        aanvraag2["status"]!.Value<string>().Should().Be(UitnodigingsStatus.WachtOpAntwoord.Status);
        aanvraag2["datumRegistratie"]!.Value<string>().Should()
            .Be(_setup.Aanvraag2AangemaaktOp.AsFormattedString());
        aanvraag2["aanvrager"]!["insz"]!.Value<string>().Should().Be(_setup.Aanvraag2.Aanvrager.Insz);
        aanvraag2["aanvrager"]!["achternaam"]!.Value<string>().Should().Be(_setup.Aanvraag2.Aanvrager.Achternaam);
        aanvraag2["aanvrager"]!["voornaam"]!.Value<string>().Should().Be(_setup.Aanvraag2.Aanvrager.Voornaam);
        aanvraag2["aanvrager"]!["e-mail"]!.Value<string>().Should().Be(_setup.Aanvraag2.Aanvrager.Email);
    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        public string VCode { get; set; }
        public AanvraagRequest Aanvraag1 { get; set; }
        public AanvraagRequest Aanvraag2 { get; set; }
        public Guid AanvraagId1 { get; set; }
        public Guid AanvraagId2 { get; set; }
        public Instant Aanvraag1AangemaaktOp { get; set; }
        public Instant Aanvraag2AangemaaktOp { get; set; }

        private readonly UitnodigingenApiClient _client;
        private UitnodigingenApiFixture _fixture;

        public Setup(UitnodigingenApiFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.Clients.Authenticated;


            var autoFixture = new AutoFixture.Fixture()
                .CustomizeAll();
            Aanvraag1 = autoFixture
                .Create<AanvraagRequest>();
            Aanvraag2 = autoFixture
                .Create<AanvraagRequest>();

            VCode = Aanvraag2.VCode = Aanvraag1.VCode;
        }

        public void Dispose()
        {
            _fixture.ResetDatabase();
        }

        public async Task InitializeAsync()
        {
            AanvraagId1 = await RegistreerAanvraag(Aanvraag1);
            Aanvraag1AangemaaktOp = _fixture.Clock.PreviousInstant;

            AanvraagId2 = await RegistreerAanvraag(Aanvraag2);
            Aanvraag2AangemaaktOp = _fixture.Clock.PreviousInstant;
        }

        private async Task<Guid> RegistreerAanvraag(AanvraagRequest aanvraagRequest)
        {
            var response = await _client.RegistreerAanvraag(aanvraagRequest).EnsureSuccessOrThrowForUitnodiging();
            return await response.ParseIdFromAanvraagResponse();
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
