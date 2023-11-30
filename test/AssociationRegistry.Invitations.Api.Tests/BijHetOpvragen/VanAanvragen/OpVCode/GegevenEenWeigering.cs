namespace AssociationRegistry.Invitations.Api.Tests.BijHetOpvragen.VanAanvragen.OpVCode;

using Aanvragen.Registreer;
using Infrastructure.Extensions;
using Autofixture;
using Fixture;
using Fixture.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenWeigering : IClassFixture<GegevenEenWeigering.Setup>
{
    private readonly TestApiClient _client;
    private readonly Setup _setup;

    public GegevenEenWeigering(TestApiFixture fixture, Setup setup)
    {
        _setup = setup;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse200()
    {
        var response = await _client.Aanvragen.GetAanvragenOpVcode(_setup.Aanvraag.VCode);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DanBevatDeBodyDeGeregistreerdeAanvraag()
    {
        var response = await _client.Aanvragen.GetAanvragenOpVcode(_setup.Aanvraag.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var token = JsonConvert.DeserializeObject<JObject>(content,
            new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
        var aanvraag = token["aanvragen"].Should().ContainSingle().Subject;
        aanvraag["aanvraagId"].Value<string>().Should().Be(_setup.AanvraagId.ToString());
        aanvraag["vCode"].Value<string>().Should().Be(_setup.Aanvraag.VCode);
        aanvraag["boodschap"].Value<string>().Should().Be(_setup.Aanvraag.Boodschap);
        aanvraag["status"].Value<string>().Should().Be(AanvraagStatus.Geweigerd.Status);
        aanvraag["datumLaatsteAanpassing"].Value<string>().Should().Be(_setup.AanvraagAanvaardOp.AsFormattedString());
        aanvraag["aanvrager"]["insz"].Value<string>().Should().Be(_setup.Aanvraag.Aanvrager.Insz);
        aanvraag["aanvrager"]["achternaam"].Value<string>().Should().Be(_setup.Aanvraag.Aanvrager.Achternaam);
        aanvraag["aanvrager"]["voornaam"].Value<string>().Should().Be(_setup.Aanvraag.Aanvrager.Voornaam);
        aanvraag["aanvrager"]["e-mail"].Value<string>().Should().Be(_setup.Aanvraag.Aanvrager.Email);
    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        public AanvraagRequest Aanvraag { get; set; }
        public Guid AanvraagId { get; set; }
        public Instant AanvraagAanvaardOp { get; set; }

        private readonly TestApiClient _client;
        private TestApiFixture _fixture;

        public Setup(TestApiFixture fixture)
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
            var response = await _client.Aanvragen.RegistreerAanvraag(Aanvraag)
                                        .EnsureSuccessOrThrowForAanvraag();

            AanvraagId = await response.ParseIdFromAanvraagResponse();

            await _client.Aanvragen.WeigerAanvraag(AanvraagId);

            AanvraagAanvaardOp = _fixture.Clock.PreviousInstant;
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
