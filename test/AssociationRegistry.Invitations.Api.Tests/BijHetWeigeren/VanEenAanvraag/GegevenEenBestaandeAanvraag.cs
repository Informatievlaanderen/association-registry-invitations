namespace AssociationRegistry.Invitations.Api.Tests.BijHetWeigeren.VanEenAanvraag;

using Aanvragen.Registreer;
using AssociationRegistry.Invitations.Api.Tests.Autofixture;
using AssociationRegistry.Invitations.Api.Tests.Fixture;
using Fixture.Extensions;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenBestaandeAanvraag : IClassFixture<GegevenEenBestaandeAanvraag.Setup>
{
    private readonly Setup _setup;
    private readonly TestApiClient _client;

    public GegevenEenBestaandeAanvraag(TestApiFixture fixture, Setup setup)
    {
        _setup = setup;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse202()
    {
        var response = await _client.Aanvragen.AanvaardAanvraag(_setup.AanvraagId);
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        public AanvraagRequest Aanvraag { get; set; }
        public Guid AanvraagId { get; set; }

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
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
