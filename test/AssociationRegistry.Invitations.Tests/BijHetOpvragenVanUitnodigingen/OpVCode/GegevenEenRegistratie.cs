using System.Net;
using AssociationRegistry.Invitations.Tests.Fixture;
using AssociationRegistry.Invitations.Uitnodingen.Requests;
using Newtonsoft.Json.Linq;

namespace AssociationRegistry.Invitations.Tests.BijHetOpvragenVanUitnodigingen.OpVCode;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenRegistratie : IClassFixture<GegevenEenRegistratie.Setup>
{
    private readonly UitnodigingenApiClient _client;
    private readonly Setup _setup;

    public GegevenEenRegistratie(UitnodigingenApiFixture fixture, Setup setup)
    {
        _setup = setup;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse200()
    {
        var response = await _client.GetUitnodigingenOpVcode("V0000001");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DanBevatDeBodyDeGeregistreerdeUitnodiging()
    {
        var response = await _client.GetUitnodigingenOpVcode(_setup.Uitnodiging.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var token = JToken.Parse(content);
        token["uitnodigingen"].Should().HaveCount(1);
        token["uitnodigingen"][0]["id"].Value<string>().Should().Be(_setup.UitnodigingsId.ToString());
    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        public UitnodigingsRequest Uitnodiging { get; set; }
        public Guid UitnodigingsId { get; set; }

        private readonly UitnodigingenApiClient _client;
        private UitnodigingenApiFixture _fixture;

        public Setup(UitnodigingenApiFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.Clients.Authenticated;

            Uitnodiging = new UitnodigingsRequest()
            {
                VCode = "V0000001"
            };
        }

        public void Dispose()
        {
            _fixture.RestDatabase();
        }

        public async Task InitializeAsync()
        {
            var response = await _client.RegistreerUitnodiging(Uitnodiging);
            var content = await response.Content.ReadAsStringAsync();
            UitnodigingsId = Guid.Parse(JToken.Parse(content)["id"]!.Value<string>()!);
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
