namespace AssociationRegistry.Invitations.Api.Tests.BijHetAanvaarden.VanEenUitnodiging;

using Autofixture;
using Fixture;
using Fixture.Extensions;
using Newtonsoft.Json.Linq;
using System.Net;
using Uitnodigingen.Registreer;
using Uitnodigingen.StatusWijziging;

[Collection(TestApiCollection.Name)]
public class GegevenGeenValidator: IClassFixture<GegevenGeenValidator.Setup>
{
    private readonly Setup _setup;
    private readonly TestApiClient _client;

    public GegevenGeenValidator(TestApiFixture fixture, Setup setup)
    {
        _setup = setup;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.Uitnodiging.AanvaardUitnodiging(_setup.UitnodigingId, new WijzigUitnodigingStatusRequest
        {
            Validator = null!,
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        var response = await _client.Uitnodiging.AanvaardUitnodiging(_setup.UitnodigingId, new WijzigUitnodigingStatusRequest
        {
            Validator = null!,
        });

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);

        token["errors"]!.ToObject<Dictionary<string, string[]>>()
                        .Should().ContainKey("validator")
                        .WhoseValue
                        .Should().ContainEquivalentOf("Validator is verplicht.");
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
