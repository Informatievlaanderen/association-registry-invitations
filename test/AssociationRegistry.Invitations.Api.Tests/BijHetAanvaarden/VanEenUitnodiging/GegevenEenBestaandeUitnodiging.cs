namespace AssociationRegistry.Invitations.Api.Tests.BijHetAanvaarden.VanEenUitnodiging;

using Autofixture;
using Fixture;
using Fixture.Extensions;
using Uitnodigingen.Registreer;
using System.Net;
using Uitnodigingen.StatusWijziging;

[Collection(TestApiCollection.Name)]
public class GegevenEenBestaandeUitnodiging : IClassFixture<GegevenEenBestaandeUitnodiging.Setup>
{
    private readonly Setup _setup;
    private readonly TestApiClient _client;

    public GegevenEenBestaandeUitnodiging(TestApiFixture fixture, Setup setup)
    {
        _setup = setup;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse202()
    {
        var response = await _client.Uitnodiging.AanvaardUitnodiging(_setup.UitnodigingId);

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        public UitnodigingsRequest Uitnodiging { get; set; }
        public Guid UitnodigingId { get; set; }
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
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
