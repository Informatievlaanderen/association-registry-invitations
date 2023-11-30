namespace AssociationRegistry.Invitations.Api.Tests.BijHetOpvragen.VanEenAanvraag;

using Aanvragen.Registreer;
using Autofixture;
using Fixture;
using Fixture.Extensions;
using Newtonsoft.Json.Linq;
using NodaTime;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenOngeldigInsz : IClassFixture<GegevenEenOngeldigInsz.Setup>
{
    private readonly Setup _setup;
    private readonly TestApiClient _client;

    public GegevenEenOngeldigInsz(TestApiFixture fixture, Setup setup)
    {
        _setup = setup;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.Aanvragen.GetAanvraagDetail("99.99.99-999.64", _setup.AanvraagId, _client);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DanBevatDeBodyEenError()
    {
        var response = await _client.Aanvragen.GetAanvraagDetail("99.99.99-999.64", _setup.AanvraagId, _client);
        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("insz")
            .WhoseValue
            .Should().ContainEquivalentOf("Deze aanvraag werd niet door deze persoon aangevraagd.");
    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        public AanvraagRequest Aanvraag { get; set; }
        public Guid AanvraagId { get; set; }
        public Instant AanvraagGeregistreerdOp { get; set; }

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
            var response = await _client.Aanvragen.RegistreerAanvraag(Aanvraag, _client)
                                        .EnsureSuccessOrThrowForAanvraag();

            AanvraagId = await response.ParseIdFromAanvraagResponse();
            AanvraagGeregistreerdOp = _fixture.Clock.PreviousInstant;
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
