using System.Net;
using AssociationRegistry.Invitations.Api.Tests.Autofixture;
using AssociationRegistry.Invitations.Api.Tests.Fixture;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Requests;
using Newtonsoft.Json.Linq;

namespace AssociationRegistry.Invitations.Api.Tests.BijHetWeigerenVanEenUitnodiging;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenReedsVerwerkteUitnodiging : IClassFixture<GegevenEenReedsVerwerkteUitnodiging.Setup>
{
    private readonly Setup _setup;
    private readonly UitnodigingenApiClient _client;

    public GegevenEenReedsVerwerkteUitnodiging(UitnodigingenApiFixture fixture, Setup setup)
    {
        _setup = setup;
        _client = fixture.Clients.Authenticated;
    }
    
    [Fact]
    public async Task DanIsDeResponse400()
    {
        foreach (var id in _setup.VerwerkteUitnodigingIds)
        {
            var response = await _client.WeigerUitnodiging(id);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);            
        }
    }


    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        foreach (var id in _setup.VerwerkteUitnodigingIds)
        {
            var response = await _client.WeigerUitnodiging(id);

            var content = await response.Content.ReadAsStringAsync();
            var token = JToken.Parse(content);
            token["errors"]!.ToObject<Dictionary<string, string[]>>()
                .Should().ContainKey("Uitnodiging")
                .WhoseValue
                .Should().ContainEquivalentOf("Deze uitnodiging is reeds verwerkt.");
        }
    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        public List<Guid> VerwerkteUitnodigingIds { get; } = new();

        private readonly UitnodigingenApiClient _client;
        private readonly UitnodigingenApiFixture _fixture;
        private readonly IFixture? _autoFixture;

        public Setup(UitnodigingenApiFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.Clients.Authenticated;

            _autoFixture = new AutoFixture.Fixture()
                .Customize(new GeldigeUitnodigingen());
        }

        public void Dispose()
        {
            _fixture.ResetDatabase();
        }

        public async Task InitializeAsync()
        {
            var response = await _client.RegistreerUitnodiging(_autoFixture.Create<UitnodigingsRequest>());
            var content = await response.Content.ReadAsStringAsync();
            var aanvaardeUitnodigingId = Guid.Parse(JToken.Parse(content)["id"]!.Value<string>()!);
            VerwerkteUitnodigingIds.Add(aanvaardeUitnodigingId);

            var tweedeUitnodiding = _autoFixture.Create<UitnodigingsRequest>();
            tweedeUitnodiding.Uitgenodigde.Insz = "02030400139";
            response = await _client.RegistreerUitnodiging(tweedeUitnodiding);
            content = await response.Content.ReadAsStringAsync();
            var geweigerdeUitnodigingId = Guid.Parse(JToken.Parse(content)["id"]!.Value<string>()!);
            VerwerkteUitnodigingIds.Add(geweigerdeUitnodigingId);
            
            await _client.AanvaardUitnodiging(aanvaardeUitnodigingId);
            await _client.WeigerUitnodiging(geweigerdeUitnodigingId);
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
