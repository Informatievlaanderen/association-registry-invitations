namespace AssociationRegistry.Invitations.Api.Tests.BijHetOpvragen.VanEenUitnodiging;

using Autofixture;
using Fixture;
using Uitnodigingen.Registreer;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenOnbekendeUitnodiginsId : IClassFixture<GegevenEenOnbekendeUitnodiginsId.Setup>
{
    private readonly TestApiClient _client;

    public GegevenEenOnbekendeUitnodiginsId(TestApiFixture fixture, Setup setup)
    {
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.Uitnodiging.GetUitnodigingsDetail(new AutoFixture.Fixture().Create<string>(), Guid.NewGuid());
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]

    public async Task DanBevatDeBodyEenError()
    {
        var response = await _client.Uitnodiging.GetUitnodigingsDetail(new AutoFixture.Fixture().Create<string>(), Guid.NewGuid());
        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("uitnodigingId")
            .WhoseValue
            .Should().ContainEquivalentOf("Deze uitnodiging is niet gekend.");
    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        private readonly TestApiClient _client;
        private readonly TestApiFixture _fixture;
        private readonly IEnumerable<UitnodigingsRequest> _uitnodigingen;

        public Setup(TestApiFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.Clients.Authenticated;


            _uitnodigingen = new AutoFixture.Fixture().CustomizeAll()
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
                await _client.Uitnodiging.RegistreerUitnodiging(request).EnsureSuccessOrThrowForUitnodiging();
            }
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
