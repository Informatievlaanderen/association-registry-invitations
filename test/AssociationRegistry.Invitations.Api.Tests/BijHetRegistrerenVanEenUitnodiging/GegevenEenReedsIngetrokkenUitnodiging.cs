using System.Net;
using AssociationRegistry.Invitations.Api.Tests.Autofixture;
using AssociationRegistry.Invitations.Api.Tests.Fixture;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Requests;
using Newtonsoft.Json.Linq;

namespace AssociationRegistry.Invitations.Api.Tests.BijHetRegistrerenVanEenUitnodiging;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenReedsIngetrokkenUitnodiging : IClassFixture<GegevenEenReedsIngetrokkenUitnodiging.Setup>
{
    private readonly UitnodigingenApiClient _client;
    private readonly UitnodigingsRequest _request;

    public GegevenEenReedsIngetrokkenUitnodiging(UitnodigingenApiFixture fixture, Setup setup)
    {
        _client = fixture.Clients.Authenticated;
        _request = new AutoFixture.Fixture()
            .CustomizeAll()
            .Create<UitnodigingsRequest>();
        _request.VCode = setup.Uitnodiging.VCode;
        _request.Uitgenodigde.Insz = setup.Uitnodiging.Uitgenodigde.Insz;
    }

    [Fact]
    public async Task DanIsDeResponse201()
    {
        var response = await _client.RegistreerUitnodiging(_request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task DanHeeftDeBodyEenIdDatEenGuidIs()
    {
        var response = await _client.RegistreerUitnodiging(_request);

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        Guid.TryParse(token["uitnodigingId"]!.Value<string>(), out _).Should().BeTrue();
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
            var response = await _client.RegistreerUitnodiging(Uitnodiging).EnsureSuccessOrThrow();
            
            UitnodigingId = await response.ParseIdFromContentString();
            await _client.TrekUitnodigingIn(UitnodigingId).EnsureSuccessOrThrow();
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
