namespace AssociationRegistry.Invitations.Api.Tests.BijHetOpvragen.VanEenAanvraag;

using Aanvragen.Registreer;
using Autofixture;
using Fixture;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenOnbekendeAanvraagId : IClassFixture<GegevenEenOnbekendeAanvraagId.Setup>
{
    private readonly TestApiClient _client;

    public GegevenEenOnbekendeAanvraagId(TestApiFixture fixture, Setup setup)
    {
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.Aanvragen.GetAanvraagDetail(new AutoFixture.Fixture().Create<string>(), Guid.NewGuid());
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DanBevatDeBodyEenError()
    {
        var response = await _client.Aanvragen.GetAanvraagDetail(new AutoFixture.Fixture().Create<string>(), Guid.NewGuid());
        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);

        token["errors"]!.ToObject<Dictionary<string, string[]>>()
                        .Should().ContainKey("aanvraagId")
                        .WhoseValue
                        .Should().ContainEquivalentOf("Deze aanvraag is niet gekend.");
    }

    public class Setup : IDisposable, IAsyncLifetime
    {
        private readonly TestApiClient _client;
        private readonly TestApiFixture _fixture;
        private readonly IEnumerable<AanvraagRequest> _aanvragen;

        public Setup(TestApiFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.Clients.Authenticated;

            _aanvragen = new AutoFixture.Fixture().CustomizeAll()
                                                     .CreateMany<AanvraagRequest>();
        }

        public void Dispose()
        {
            _fixture.ResetDatabase();
        }

        public async Task InitializeAsync()
        {
            foreach (var request in _aanvragen)
            {
                await _client.Aanvragen.RegistreerAanvraag(request).EnsureSuccessOrThrowForAanvraag();
            }
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
