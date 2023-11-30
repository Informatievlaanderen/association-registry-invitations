namespace AssociationRegistry.Invitations.Api.Tests.BijHetIntrekken.VanEenUitnodiging;

using Autofixture;
using Fixture;
using Uitnodigingen.Registreer;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenReedsIngetrokkenUitnodiging : IClassFixture<GegevenEenReedsIngetrokkenUitnodiging.Setup>
{
    private readonly Setup _setup;
    private readonly UitnodigingenApiClient _client;

    public GegevenEenReedsIngetrokkenUitnodiging(UitnodigingenApiFixture fixture, Setup setup)
    {
        _setup = setup;
        _client = fixture.Clients.Authenticated;
    }
    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.TrekUitnodigingIn(_setup.UitnodigingId);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        var response = await _client.TrekUitnodigingIn(_setup.UitnodigingId);

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("uitnodiging")
            .WhoseValue
            .Should().ContainEquivalentOf(Resources.IntrekkenUitnodigingOnmogelijk);
    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        public UitnodigingsRequest Uitnodiging { get; set; }
        public Guid UitnodigingId { get; set; }

        private readonly UitnodigingenApiClient _client;
        private UitnodigingenApiFixture _fixture;

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
            var response = await _client.RegistreerUitnodiging(Uitnodiging)
                .EnsureSuccessOrThrowForUitnodiging();
            
            UitnodigingId = await response.ParseIdFromUitnodigingResponse();

            await _client.TrekUitnodigingIn(UitnodigingId);
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
