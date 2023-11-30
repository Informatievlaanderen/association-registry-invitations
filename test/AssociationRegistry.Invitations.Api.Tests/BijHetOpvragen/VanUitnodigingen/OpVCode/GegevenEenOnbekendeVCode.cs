namespace AssociationRegistry.Invitations.Api.Tests.BijHetOpvragen.VanUitnodigingen.OpVCode;

using Autofixture;
using Fixture;
using Uitnodigingen.Registreer;
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
        var response = await _client.Uitnodiging.GetUitnodigingenOpVcode(onbekendeVcode, _client);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [MemberData(nameof(Data))]
    public async Task DanBevatDeBodyDeGeenUitnodigingen(string onbekendeVcode)
    {
        var response = await _client.Uitnodiging.GetUitnodigingenOpVcode(onbekendeVcode, _client);
        var content = await response.Content.ReadAsStringAsync();

        var token = JToken.Parse(content);
        token["uitnodigingen"].Should()
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
                await _client.Uitnodiging.RegistreerUitnodiging(request, _client).EnsureSuccessOrThrowForUitnodiging();
            }
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
