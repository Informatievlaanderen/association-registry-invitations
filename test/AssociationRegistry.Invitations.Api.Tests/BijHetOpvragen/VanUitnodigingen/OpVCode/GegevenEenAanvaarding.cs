namespace AssociationRegistry.Invitations.Api.Tests.BijHetOpvragen.VanUitnodigingen.OpVCode;

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
        var response = await _client.Uitnodiging.GetUitnodigingenOpVcode(_setup.Uitnodiging.VCode);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DanBevatDeBodyDeGeregistreerdeUitnodiging()
    {
        var response = await _client.Uitnodiging.GetUitnodigingenOpVcode(_setup.Uitnodiging.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var token = JsonConvert.DeserializeObject<JObject>(content,
            new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
        var uitnodiging = token["uitnodigingen"].Should().ContainSingle().Subject;
        uitnodiging["uitnodigingId"].Value<string>().Should().Be(_setup.UitnodigingId.ToString());
        uitnodiging["vCode"].Value<string>().Should().Be(_setup.Uitnodiging.VCode);
        uitnodiging["boodschap"].Value<string>().Should().Be(_setup.Uitnodiging.Boodschap);
        uitnodiging["status"].Value<string>().Should().Be(UitnodigingsStatus.Aanvaard.Status);
        uitnodiging["datumLaatsteAanpassing"].Value<string>().Should()
            .Be(_setup.UitnodigingAanvaardOp.AsFormattedString());
        uitnodiging["uitnodiger"]["vertegenwoordigerId"].Value<int>().Should()
            .Be(_setup.Uitnodiging.Uitnodiger.VertegenwoordigerId);
        uitnodiging["uitgenodigde"]["insz"].Value<string>().Should().Be(_setup.Uitnodiging.Uitgenodigde.Insz);
        uitnodiging["uitgenodigde"]["achternaam"].Value<string>().Should().Be(_setup.Uitnodiging.Uitgenodigde.Achternaam);
        uitnodiging["uitgenodigde"]["voornaam"].Value<string>().Should().Be(_setup.Uitnodiging.Uitgenodigde.Voornaam);
        uitnodiging["uitgenodigde"]["e-mail"].Value<string>().Should().Be(_setup.Uitnodiging.Uitgenodigde.Email);
    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        public UitnodigingsRequest Uitnodiging { get; set; }
        public Guid UitnodigingId { get; set; }
        public Instant UitnodigingAanvaardOp { get; set; }

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
            await _client.Uitnodiging.AanvaardUitnodiging(UitnodigingId);
            UitnodigingAanvaardOp = _fixture.Clock.PreviousInstant;
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}