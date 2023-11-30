namespace AssociationRegistry.Invitations.Api.Tests.BijHetRegisteren.VanEenAanvraag.MetOngeldigeData;

using Aanvragen.Registreer;
using Autofixture;
using Fixture;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenAanvragerMetEenOngeldigeEmail : IDisposable
{
    private readonly UitnodigingenApiClient _client;
    private readonly UitnodigingenApiFixture _fixture;
    private readonly AanvraagRequest _request;

    public GegevenEenAanvragerMetEenOngeldigeEmail(UitnodigingenApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
        _request = new AutoFixture.Fixture()
            .CustomizeAll()
            .Create<AanvraagRequest>();
        _request.Aanvrager.Email = "GeenGeldigeEmail";
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.RegistreerAanvraag(_request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        var response = await _client.RegistreerAanvraag(_request);

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("aanvrager.Email")
            .WhoseValue
            .Should().ContainEquivalentOf("Email is ongeldig.");
    }

    public void Dispose()
    {
        _fixture.ResetDatabase();
    }
}
