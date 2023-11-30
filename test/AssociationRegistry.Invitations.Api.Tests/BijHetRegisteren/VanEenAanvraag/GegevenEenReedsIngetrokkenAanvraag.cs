namespace AssociationRegistry.Invitations.Api.Tests.BijHetRegisteren.VanEenAanvraag;

using Aanvragen.Registreer;
using Autofixture;
using Fixture;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenReedsIngetrokkenAanvraag : IClassFixture<GegevenEenReedsIngetrokkenAanvraag.Setup>
{
    private readonly Setup _setup;

    public GegevenEenReedsIngetrokkenAanvraag(Setup setup)
    {
        _setup = setup;
    }

    [Fact]
    public void DanIsDeResponse201()
    {
        _setup.ActResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task DanHeeftDeBodyEenIdDatEenGuidIs()
    {
        var content = await _setup.ActResponse.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        Guid.TryParse(token["aanvraagId"]!.Value<string>(), out _).Should().BeTrue();
    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        public AanvraagRequest Aanvraag { get; set; }
        public Guid AanvraagId { get; set; }

        private readonly UitnodigingenApiClient _client;
        private UitnodigingenApiFixture _fixture;
        public HttpResponseMessage ActResponse { get; private set; }

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
            var response = await _client.RegistreerAanvraag(Aanvraag).EnsureSuccessOrThrowForUitnodiging();

            AanvraagId = await response.ParseIdFromAanvraagResponse();
            await _client.TrekAanvraagIn(AanvraagId).EnsureSuccessOrThrowForUitnodiging();

            var request = new AutoFixture.Fixture()
                .CustomizeAll()
                .Create<AanvraagRequest>();
            request.VCode = Aanvraag.VCode;
            request.Aanvrager.Insz = Aanvraag.Aanvrager.Insz;
            ActResponse = await _client.RegistreerAanvraag(request);
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
