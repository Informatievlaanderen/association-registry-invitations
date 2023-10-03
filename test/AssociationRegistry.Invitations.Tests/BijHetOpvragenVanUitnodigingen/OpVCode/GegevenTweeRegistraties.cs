using System.Net;
using AssociationRegistry.Invitations.Tests.Autofixture;
using AssociationRegistry.Invitations.Tests.Fixture;
using AssociationRegistry.Invitations.Uitnodingen.Requests;
using Newtonsoft.Json.Linq;

namespace AssociationRegistry.Invitations.Tests.BijHetOpvragenVanUitnodigingen.OpVCode;

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

        var token = JToken.Parse(content);
        var uitnodiging1 = token["uitnodigingen"].Should()
            .ContainSingle(u => u["id"].Value<string>() == _setup.UitnodigingsId1.ToString()).Subject;
        uitnodiging1["vCode"].Value<string>().Should().Be(_setup.VCode);
        uitnodiging1["boodschap"].Value<string>().Should().Be(_setup.Uitnodiging1.Boodschap);
        uitnodiging1["uitnodiger"]["vertegenwoordigerId"].Value<int>().Should()
            .Be(_setup.Uitnodiging1.Uitnodiger.VertegenwoordigerId);
        uitnodiging1["uitgenodigde"]["insz"].Value<string>().Should().Be(_setup.Uitnodiging1.Uitgenodigde.Insz);
        uitnodiging1["uitgenodigde"]["naam"].Value<string>().Should().Be(_setup.Uitnodiging1.Uitgenodigde.Naam);
        uitnodiging1["uitgenodigde"]["voornaam"].Value<string>().Should().Be(_setup.Uitnodiging1.Uitgenodigde.Voornaam);

        var uitnodiging2 = token["uitnodigingen"].Should()
            .ContainSingle(u => u["id"].Value<string>() == _setup.UitnodigingsId2.ToString()).Subject;
        uitnodiging2["vCode"].Value<string>().Should().Be(_setup.VCode);
        uitnodiging2["boodschap"].Value<string>().Should().Be(_setup.Uitnodiging2.Boodschap);
        uitnodiging2["uitnodiger"]["vertegenwoordigerId"].Value<int>().Should()
            .Be(_setup.Uitnodiging2.Uitnodiger.VertegenwoordigerId);
        uitnodiging2["uitgenodigde"]["insz"].Value<string>().Should().Be(_setup.Uitnodiging2.Uitgenodigde.Insz);
        uitnodiging2["uitgenodigde"]["naam"].Value<string>().Should().Be(_setup.Uitnodiging2.Uitgenodigde.Naam);
        uitnodiging2["uitgenodigde"]["voornaam"].Value<string>().Should().Be(_setup.Uitnodiging2.Uitgenodigde.Voornaam);

    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        public string VCode { get; set; }
        public UitnodigingsRequest Uitnodiging1 { get; set; }
        public UitnodigingsRequest Uitnodiging2 { get; set; }
        public Guid UitnodigingsId1 { get; set; }
        public Guid UitnodigingsId2 { get; set; }

        private readonly UitnodigingenApiClient _client;
        private UitnodigingenApiFixture _fixture;

        public Setup(UitnodigingenApiFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.Clients.Authenticated;

            VCode = "V0102030";

            Uitnodiging1 = new UitnodigingenFixture().Build<UitnodigingsRequest>()
                .With(u => u.VCode, VCode)
                .Create();
            Uitnodiging2 = new UitnodigingenFixture().Build<UitnodigingsRequest>()
                .With(u => u.VCode, VCode)
                .Create();
        }

        public void Dispose()
        {
            _fixture.RestDatabase();
        }

        public async Task InitializeAsync()
        {
            UitnodigingsId1 = await RegistreerUitnodiging(Uitnodiging1);
            UitnodigingsId2 = await RegistreerUitnodiging(Uitnodiging2);
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
