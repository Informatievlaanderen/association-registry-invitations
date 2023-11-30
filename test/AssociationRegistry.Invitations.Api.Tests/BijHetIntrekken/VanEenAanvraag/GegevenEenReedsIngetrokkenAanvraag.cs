namespace AssociationRegistry.Invitations.Api.Tests.BijHetIntrekken.VanEenAanvraag;

using Aanvragen.Registreer;
using Autofixture;
using Fixture;
using Fixture.Extensions;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenReedsIngetrokkenAanvraag : IClassFixture<GegevenEenReedsIngetrokkenAanvraag.Setup>
{
    private readonly Setup _setup;
    private readonly TestApiClient _client;

    public GegevenEenReedsIngetrokkenAanvraag(TestApiFixture fixture, Setup setup)
    {
        _setup = setup;
        _client = fixture.Clients.Authenticated;
    }
    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.Aanvragen.TrekAanvraagIn(_setup.AanvraagId);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        var response = await _client.Aanvragen.TrekAanvraagIn(_setup.AanvraagId);

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("aanvraag")
            .WhoseValue
            .Should().ContainEquivalentOf(Resources.IntrekkenAanvraagOnmogelijk);
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

            await _client.Aanvragen.TrekAanvraagIn(AanvraagId);
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
