namespace AssociationRegistry.Invitations.Api.Tests.BijHetOpvragen.VanEenAanvraag;

using Aanvragen.Registreer;
using Autofixture;
using Fixture;
using Newtonsoft.Json.Linq;
using NodaTime;
using System.Net;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenOngeldigInsz : IClassFixture<GegevenEenOngeldigInsz.Setup>
{
    private readonly Setup _setup;
    private readonly UitnodigingenApiClient _client;

    public GegevenEenOngeldigInsz(UitnodigingenApiFixture fixture, Setup setup)
    {
        _setup = setup;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.GetAanvraagDetail("99.99.99-999.64", _setup.AanvraagId);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DanBevatDeBodyEenError()
    {
        var response = await _client.GetAanvraagDetail("99.99.99-999.64", _setup.AanvraagId);
        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("insz")
            .WhoseValue
            .Should().ContainEquivalentOf("Deze aanvraag is niet voor deze persoon bestemd.");
    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        public AanvraagRequest Aanvraag { get; set; }
        public Guid AanvraagId { get; set; }
        public Instant AanvraagGeregistreerdOp { get; set; }

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
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
