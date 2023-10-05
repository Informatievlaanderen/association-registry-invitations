using System.Net;
using AssociationRegistry.Invitations.Api.Tests.Autofixture;
using AssociationRegistry.Invitations.Api.Tests.Fixture;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Requests;
using Newtonsoft.Json.Linq;

namespace AssociationRegistry.Invitations.Api.Tests.BijHetOpvragenvanEenUitnodiging;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenOnbekendeUitnodiginsId : IClassFixture<GegevenEenOnbekendeUitnodiginsId.Setup>
{
    private readonly UitnodigingenApiClient _client;

    public GegevenEenOnbekendeUitnodiginsId(UitnodigingenApiFixture fixture, Setup setup)
    {
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.GetUitnodigingsDetail(new AutoFixture.Fixture().Create<string>(), Guid.NewGuid());
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]

    public async Task DanBevatDeBodyEenError()
    {
        var response = await _client.GetUitnodigingsDetail(new AutoFixture.Fixture().Create<string>(), Guid.NewGuid());
        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("uitnodigingsId")
            .WhoseValue
            .Should().ContainEquivalentOf("Deze uitnodiging is niet gekend.");
    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        private readonly UitnodigingenApiClient _client;
        private readonly UitnodigingenApiFixture _fixture;
        private readonly IEnumerable<UitnodigingsRequest> _uitnodigingen;

        public Setup(UitnodigingenApiFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.Clients.Authenticated;


            _uitnodigingen = new AutoFixture.Fixture()
                .Customize(new GeldigeUitnodigingen())
                .CreateMany<UitnodigingsRequest>();
        }

        public void Dispose()
        {
            _fixture.ResetDatabase();
        }

        public async Task InitializeAsync()
        {
            foreach (var request in _uitnodigingen)
            {
                await _client.RegistreerUitnodiging(request);
            }
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
