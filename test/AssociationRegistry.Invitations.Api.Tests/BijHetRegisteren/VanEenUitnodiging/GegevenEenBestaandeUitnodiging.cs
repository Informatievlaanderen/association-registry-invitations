namespace AssociationRegistry.Invitations.Api.Tests.BijHetRegisteren.VanEenUitnodiging;

using Autofixture;
using Fixture;
using Fixture.Extensions;
using Uitnodigingen.Registreer;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenBestaandeUitnodiging : IClassFixture<GegevenEenBestaandeUitnodiging.Setup>
{
    private readonly TestApiClient _client;
    private readonly UitnodigingsRequest _request;

    public GegevenEenBestaandeUitnodiging(TestApiFixture fixture, Setup setup)
    {
        _client = fixture.Clients.Authenticated;
        _request = setup.Uitnodiging; 
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.Uitnodiging.RegistreerUitnodiging(_request, _client);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        var response = await _client.Uitnodiging.RegistreerUitnodiging(_request, _client);

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("uitnodiging")
            .WhoseValue
            .Should().ContainEquivalentOf("Deze persoon is reeds uitgenodigd.");
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
            var response = await _client.Uitnodiging.RegistreerUitnodiging(Uitnodiging, _client)
                                        .EnsureSuccessOrThrowForUitnodiging();
            
            UitnodigingId = await response.ParseIdFromUitnodigingResponse();
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
