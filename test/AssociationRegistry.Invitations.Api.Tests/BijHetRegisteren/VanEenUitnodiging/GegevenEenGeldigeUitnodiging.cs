namespace AssociationRegistry.Invitations.Api.Tests.BijHetRegisteren.VanEenUitnodiging;

using Autofixture;
using Fixture;
using Uitnodigingen.Registreer;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenGeldigeUitnodiging : IDisposable
{
    private readonly TestApiClient _client;
    private readonly TestApiFixture _fixture;
    private readonly UitnodigingsRequest _request;

    public GegevenEenGeldigeUitnodiging(TestApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
        _request = new AutoFixture.Fixture()
            .CustomizeAll()
            .Create<UitnodigingsRequest>();
    }

    [Fact]
    public async Task DanIsDeResponse201()
    {
        var response = await _client.Uitnodiging.RegistreerUitnodiging(_request, _client);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task DanHeeftDeBodyEenIdDatEenGuidIs()
    {
        var response = await _client.Uitnodiging.RegistreerUitnodiging(_request, _client);

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        Guid.TryParse(token["uitnodigingId"]!.Value<string>(), out _).Should().BeTrue();
    }

    public void Dispose()
    {
        _fixture.ResetDatabase();
    }
}
