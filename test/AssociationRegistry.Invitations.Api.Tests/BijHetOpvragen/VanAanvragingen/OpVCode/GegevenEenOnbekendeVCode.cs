namespace AssociationRegistry.Invitations.Api.Tests.BijHetOpvragen.VanAanvragingen.OpVCode;

using Aanvragen.Registreer;
using Autofixture;
using Fixture;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenOnbekendeVCode : IClassFixture<GegevenEenOnbekendeVCode.Setup>
{
    private readonly UitnodigingenApiClient _client;

    public GegevenEenOnbekendeVCode(UitnodigingenApiFixture fixture, Setup setup)
    {
        _client = fixture.Clients.Authenticated;
    }

    [Theory]
    [MemberData(nameof(Data))]
    public async Task DanIsDeResponse200(string onbekendeVcode)
    {
        var response = await _client.GetAanvragingenOpVcode(onbekendeVcode);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [MemberData(nameof(Data))]
    public async Task DanBevatDeBodyDeGeenAanvragingen(string onbekendeVcode)
    {
        var response = await _client.GetAanvragingenOpVcode(onbekendeVcode);
        var content = await response.Content.ReadAsStringAsync();

        var token = JToken.Parse(content);
        token["aanvragingen"].Should()
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
        private readonly UitnodigingenApiClient _client;
        private readonly UitnodigingenApiFixture _fixture;
        private readonly IEnumerable<AanvraagRequest> _aanvragingen;

        public Setup(UitnodigingenApiFixture fixture)
        {
            _fixture = fixture;
            _client = fixture.Clients.Authenticated;


            _aanvragingen = new AutoFixture.Fixture().CustomizeAll()
                .CreateMany<AanvraagRequest>();
        }

        public void Dispose()
        {
            _fixture.ResetDatabase();
        }

        public async Task InitializeAsync()
        {
            foreach (var request in _aanvragingen)
            {
                await _client.RegistreerAanvraag(request).EnsureSuccessOrThrowForAanvraag();
            }
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
