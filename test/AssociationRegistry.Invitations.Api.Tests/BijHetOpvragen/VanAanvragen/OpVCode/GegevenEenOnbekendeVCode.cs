namespace AssociationRegistry.Invitations.Api.Tests.BijHetOpvragen.VanAanvragen.OpVCode;

using Aanvragen.Registreer;
using Autofixture;
using Fixture;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenOnbekendeVCode : IClassFixture<GegevenEenOnbekendeVCode.Setup>
{
    private readonly TestApiClient _client;

    public GegevenEenOnbekendeVCode(TestApiFixture fixture, Setup setup)
    {
        _client = fixture.Clients.Authenticated;
    }

    [Theory]
    [MemberData(nameof(Data))]
    public async Task DanIsDeResponse200(string onbekendeVcode)
    {
        var response = await _client.Aanvragen.GetAanvragenOpVcode(onbekendeVcode);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [MemberData(nameof(Data))]
    public async Task DanBevatDeBodyDeGeenAanvragen(string onbekendeVcode)
    {
        var response = await _client.Aanvragen.GetAanvragenOpVcode(onbekendeVcode);
        var content = await response.Content.ReadAsStringAsync();

        var token = JToken.Parse(content);
        token["aanvragen"].Should()
            .BeEmpty();
    }

    public static IEnumerable<object[]> Data
    {
        get
        {
            yield return new object[] { "blablabla" };
            yield return new object[] { "OVO000001" };
        }
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
