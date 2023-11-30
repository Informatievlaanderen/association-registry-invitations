namespace AssociationRegistry.Invitations.Api.Tests.BijHetRegisteren.VanEenUitnodiging;

using Autofixture;
using Fixture;
using Uitnodigingen.Registreer;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenReedsIngetrokkenUitnodiging : IClassFixture<GegevenEenReedsIngetrokkenUitnodiging.Setup>
{
    private readonly Setup _setup;

    public GegevenEenReedsIngetrokkenUitnodiging(Setup setup)
    {
        _setup = setup;
    }

    [Fact]
    public async Task DanIsDeResponse201()
    {
        _setup.ActResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task DanHeeftDeBodyEenIdDatEenGuidIs()
    {
        var content = await _setup.ActResponse.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        Guid.TryParse(token["uitnodigingId"]!.Value<string>(), out _).Should().BeTrue();
    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        public UitnodigingsRequest Uitnodiging { get; set; }
        public Guid UitnodigingId { get; set; }

        private readonly UitnodigingenApiClient _client;
        private UitnodigingenApiFixture _fixture;
        public HttpResponseMessage ActResponse { get; private set; }

        public Setup(UitnodigingenApiFixture fixture)
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
            var response = await _client.RegistreerUitnodiging(Uitnodiging).EnsureSuccessOrThrowForUitnodiging();
            
            UitnodigingId = await response.ParseIdFromUitnodigingResponse();
            await _client.TrekUitnodigingIn(UitnodigingId).EnsureSuccessOrThrowForUitnodiging();
            
            var request = new AutoFixture.Fixture()
                .CustomizeAll()
                .Create<UitnodigingsRequest>();
            request.VCode = Uitnodiging.VCode;
            request.Uitgenodigde.Insz = Uitnodiging.Uitgenodigde.Insz;
            ActResponse = await _client.RegistreerUitnodiging(request);
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
