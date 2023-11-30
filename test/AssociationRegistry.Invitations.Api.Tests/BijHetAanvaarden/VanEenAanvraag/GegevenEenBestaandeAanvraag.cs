namespace AssociationRegistry.Invitations.Api.Tests.BijHetAanvaarden.VanEenAanvraag;

using Aanvragen.Registreer;
using Autofixture;
using Fixture;
using System.Net;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenBestaandeAanvraag : IClassFixture<GegevenEenBestaandeAanvraag.Setup>
{
    private readonly Setup _setup;
    private readonly UitnodigingenApiClient _client;

    public GegevenEenBestaandeAanvraag(UitnodigingenApiFixture fixture, Setup setup)
    {
        _setup = setup;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse202()
    {
        var response = await _client.AanvaardAanvraag(_setup.AanvraagId);
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        public AanvraagRequest Aanvraag { get; set; }
        public Guid AanvraagId { get; set; }

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

        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
