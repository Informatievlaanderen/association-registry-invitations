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
public class GegevenEenIntrekking : IClassFixture<GegevenEenIntrekking.Setup>
{
    private readonly UitnodigingenApiClient _client;
    private readonly Setup _setup;

    public GegevenEenIntrekking(UitnodigingenApiFixture fixture, Setup setup)
    {
        _setup = setup;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse200()
    {
        var response = await _client.GetAanvragingenOpVcode(_setup.Aanvraag.VCode);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DanBevatDeBodyDeAanvraging()
    {
        var response = await _client.GetAanvragingenOpVcode(_setup.Aanvraag.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var token = JsonConvert.DeserializeObject<JObject>(content,
            new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
        var aanvraag = token["aanvragingen"].Should().ContainSingle().Subject;
        aanvraag["aanvraagId"].Value<string>().Should().Be(_setup.AanvraagId.ToString());
        aanvraag["vCode"].Value<string>().Should().Be(_setup.Aanvraag.VCode);
        aanvraag["boodschap"].Value<string>().Should().Be(_setup.Aanvraag.Boodschap);
        aanvraag["status"].Value<string>().Should().Be(UitnodigingsStatus.Ingetrokken.Status);
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

            await _client.TrekAanvraagIn(AanvraagId);

            AanvraagAanvaardOp = _fixture.Clock.PreviousInstant;
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
