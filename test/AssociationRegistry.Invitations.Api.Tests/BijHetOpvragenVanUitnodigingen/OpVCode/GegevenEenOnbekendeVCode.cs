using System.Net;
using AssociationRegistry.Invitations.Api.Tests.Autofixture;
using AssociationRegistry.Invitations.Api.Tests.Fixture;
using AssociationRegistry.Invitations.Api.Uitnodingen.Requests;
using Newtonsoft.Json.Linq;

namespace AssociationRegistry.Invitations.Api.Tests.BijHetOpvragenVanUitnodigingen.OpVCode;

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
        var response = await _client.GetUitnodigingenOpVcode(onbekendeVcode);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [MemberData(nameof(Data))]
    public async Task DanBevatDeBodyDeGeenUitnodigingen(string onbekendeVcode)
    {
        var response = await _client.GetUitnodigingenOpVcode(onbekendeVcode);
        var content = await response.Content.ReadAsStringAsync();

        var token = JToken.Parse(content);
        token["uitnodigingen"].Should()
            .BeEmpty();
    }

    public static IEnumerable<object[]> Data
    {
        get
        {
            yield return new object[] { "" };
            yield return new object[] { null! };
            yield return new object[] { "blablabla" };
            yield return new object[] { "OVO000001" };
        }
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
