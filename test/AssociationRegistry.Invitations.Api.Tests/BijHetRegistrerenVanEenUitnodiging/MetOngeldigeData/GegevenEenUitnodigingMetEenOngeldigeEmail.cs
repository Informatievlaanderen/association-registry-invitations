using System.Net;
using AssociationRegistry.Invitations.Api.Tests.Autofixture;
using AssociationRegistry.Invitations.Api.Tests.Fixture;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Registreer;
using Newtonsoft.Json.Linq;

namespace AssociationRegistry.Invitations.Api.Tests.BijHetRegistrerenVanEenUitnodiging.MetOngeldigeData;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenUitnodigingMetEenOngeldigeEmail : IDisposable
{
    private readonly UitnodigingenApiClient _client;
    private readonly UitnodigingenApiFixture _fixture;
    private readonly UitnodigingsRequest _request;

    public GegevenEenUitnodigingMetEenOngeldigeEmail(UitnodigingenApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
        _request = new AutoFixture.Fixture()
            .CustomizeAll()
            .Create<UitnodigingsRequest>();
        _request.Uitgenodigde.Email = "GeenGeldigeEmail";
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.RegistreerUitnodiging(_request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        var response = await _client.RegistreerUitnodiging(_request);

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("uitgenodigde.Email")
            .WhoseValue
            .Should().ContainEquivalentOf("Email is ongeldig.");
    }

    public void Dispose()
    {
        _fixture.ResetDatabase();
    }
}
