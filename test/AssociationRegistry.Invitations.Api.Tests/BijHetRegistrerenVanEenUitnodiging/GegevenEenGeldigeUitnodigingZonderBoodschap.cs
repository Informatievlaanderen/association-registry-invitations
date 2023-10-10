using System.Net;
using AssociationRegistry.Invitations.Api.Tests.Autofixture;
using AssociationRegistry.Invitations.Api.Tests.Fixture;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Requests;
using Newtonsoft.Json.Linq;
using static System.String;

namespace AssociationRegistry.Invitations.Api.Tests.BijHetRegistrerenVanEenUitnodiging;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenGeldigeUitnodigingZonderBoodschap : IDisposable
{
    private readonly UitnodigingenApiClient _client;
    private readonly UitnodigingenApiFixture _fixture;
    private readonly UitnodigingsRequest _request;

    public GegevenEenGeldigeUitnodigingZonderBoodschap(UitnodigingenApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
        _request = new AutoFixture.Fixture()
            .CustomizeAll()
            .Create<UitnodigingsRequest>();
        _request.Boodschap = string.Empty;
    }

    [Fact]
    public async Task DanIsDeResponse201()
    {
        var response = await _client.RegistreerUitnodiging(_request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task DanHeeftDeBodyEenIdDatEenGuidIs()
    {
        var response = await _client.RegistreerUitnodiging(_request);

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        Guid.TryParse(token["id"]!.Value<string>(), out _).Should().BeTrue();
    }

    public void Dispose()
    {
        _fixture.ResetDatabase();
    }
}
