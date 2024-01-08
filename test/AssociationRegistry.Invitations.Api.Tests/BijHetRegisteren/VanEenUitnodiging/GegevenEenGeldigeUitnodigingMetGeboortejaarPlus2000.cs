namespace AssociationRegistry.Invitations.Api.Tests.BijHetRegisteren.VanEenUitnodiging;

using Autofixture;
using Fixture;
using Uitnodigingen.Registreer;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenGeldigeUitnodigingMetGeboortejaarPlus2000 : IDisposable
{
    private readonly TestApiClient _client;
    private readonly TestApiFixture _fixture;
    private readonly UitnodigingsRequest _request;

    public GegevenEenGeldigeUitnodigingMetGeboortejaarPlus2000(TestApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
        _request = new AutoFixture.Fixture()
            .CustomizeAll()
            .Create<UitnodigingsRequest>();

        _request.Uitgenodigde.Insz = "03111300268";
    }

    [Fact]
    public async Task DanIsDeResponse201()
    {
        var response = await _client.Uitnodiging.RegistreerUitnodiging(_request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task DanHeeftDeBodyEenIdDatEenGuidIs()
    {
        var response = await _client.Uitnodiging.RegistreerUitnodiging(_request);

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        Guid.TryParse(token["uitnodigingId"]!.Value<string>(), out _).Should().BeTrue();
    }

    public void Dispose()
    {
        _fixture.ResetDatabase();
    }
}
