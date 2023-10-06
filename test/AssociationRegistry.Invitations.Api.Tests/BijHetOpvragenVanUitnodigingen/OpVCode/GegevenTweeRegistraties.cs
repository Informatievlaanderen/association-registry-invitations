using System.Globalization;
using System.Net;
using AssociationRegistry.Invitations.Api.Tests.Autofixture;
using AssociationRegistry.Invitations.Api.Tests.Fixture;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Requests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime;

namespace AssociationRegistry.Invitations.Api.Tests.BijHetOpvragenVanUitnodigingen.OpVCode;

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
        var response = await _client.GetUitnodigingenOpVcode("V0000001");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DanBevatDeBodyDeGeregistreerdeUitnodiging()
    {
        var response = await _client.GetUitnodigingenOpVcode(_setup.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var token = JsonConvert.DeserializeObject<JObject>(content,
            new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
        
        var uitnodiging1 = token!["uitnodigingen"].Should()
            .ContainSingle(u => u["id"]!.Value<string>() == _setup.UitnodigingsId1.ToString()).Subject;
        uitnodiging1["vCode"]!.Value<string>().Should().Be(_setup.VCode);
        uitnodiging1["boodschap"]!.Value<string>().Should().Be(_setup.Uitnodiging1.Boodschap);
        uitnodiging1["status"]!.Value<string>().Should().Be(UitnodigingsStatus.WachtOpAntwoord.Status);
        uitnodiging1["datumRegistratie"]!.Value<string>().Should()
            .Be(_setup.Uitnodiging1AangemaaktOp.ToString("g", CultureInfo.InvariantCulture));
        uitnodiging1["uitnodiger"]!["vertegenwoordigerId"]!.Value<int>().Should()
            .Be(_setup.Uitnodiging1.Uitnodiger.VertegenwoordigerId);
        uitnodiging1["uitgenodigde"]!["insz"]!.Value<string>().Should().Be(_setup.Uitnodiging1.Uitgenodigde.Insz);
        uitnodiging1["uitgenodigde"]!["naam"]!.Value<string>().Should().Be(_setup.Uitnodiging1.Uitgenodigde.Naam);
        uitnodiging1["uitgenodigde"]!["voornaam"]!.Value<string>().Should().Be(_setup.Uitnodiging1.Uitgenodigde.Voornaam);
        uitnodiging1["uitgenodigde"]!["email"]!.Value<string>().Should().Be(_setup.Uitnodiging1.Uitgenodigde.Email);

        var uitnodiging2 = token["uitnodigingen"].Should()
            .ContainSingle(u => u["id"]!.Value<string>() == _setup.UitnodigingsId2.ToString()).Subject;
        uitnodiging2["vCode"]!.Value<string>().Should().Be(_setup.VCode);
        uitnodiging2["boodschap"]!.Value<string>().Should().Be(_setup.Uitnodiging2.Boodschap);
        uitnodiging2["status"]!.Value<string>().Should().Be(UitnodigingsStatus.WachtOpAntwoord.Status);
        uitnodiging2["datumRegistratie"]!.Value<string>().Should()
            .Be(_setup.Uitnodiging2AangemaaktOp.ToString("g", CultureInfo.InvariantCulture));
        uitnodiging2["uitnodiger"]!["vertegenwoordigerId"]!.Value<int>().Should()
            .Be(_setup.Uitnodiging2.Uitnodiger.VertegenwoordigerId);
        uitnodiging2["uitgenodigde"]!["insz"]!.Value<string>().Should().Be(_setup.Uitnodiging2.Uitgenodigde.Insz);
        uitnodiging2["uitgenodigde"]!["naam"]!.Value<string>().Should().Be(_setup.Uitnodiging2.Uitgenodigde.Naam);
        uitnodiging2["uitgenodigde"]!["voornaam"]!.Value<string>().Should().Be(_setup.Uitnodiging2.Uitgenodigde.Voornaam);
        uitnodiging2["uitgenodigde"]!["email"]!.Value<string>().Should().Be(_setup.Uitnodiging2.Uitgenodigde.Email);
    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        public string VCode { get; set; }
        public UitnodigingsRequest Uitnodiging1 { get; set; }
        public UitnodigingsRequest Uitnodiging2 { get; set; }
        public Guid UitnodigingsId1 { get; set; }
        public Guid UitnodigingsId2 { get; set; }
        public Instant Uitnodiging1AangemaaktOp { get; set; }
        public Instant Uitnodiging2AangemaaktOp { get; set; }

        private readonly UitnodigingenApiClient _client;
        private UitnodigingenApiFixture _fixture;

        public Setup(UitnodigingenApiFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.Clients.Authenticated;

            VCode = "V0102030";

            Uitnodiging1 = new AutoFixture.Fixture()
                .Customize(new GeldigeUitnodigingen(vCode: VCode, insz: "01020312316"))
                .Create<UitnodigingsRequest>();
            Uitnodiging2 = new AutoFixture.Fixture()
                .Customize(new GeldigeUitnodigingen(vCode: VCode, insz: "01020312415"))
                .Create<UitnodigingsRequest>();
        }

        public void Dispose()
        {
            _fixture.ResetDatabase();
        }

        public async Task InitializeAsync()
        {
            UitnodigingsId1 = await RegistreerUitnodiging(Uitnodiging1);
            Uitnodiging1AangemaaktOp = _fixture.Clock.PreviousInstant;
            UitnodigingsId2 = await RegistreerUitnodiging(Uitnodiging2);
            Uitnodiging2AangemaaktOp = _fixture.Clock.PreviousInstant;
        }

        private async Task<Guid> RegistreerUitnodiging(UitnodigingsRequest uitnodigingsRequest)
        {
            var response = await _client.RegistreerUitnodiging(uitnodigingsRequest);
            var content = await response.Content.ReadAsStringAsync();
            return Guid.Parse(JToken.Parse(content)["id"]!.Value<string>()!);
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
