namespace AssociationRegistry.Invitations.Api.Tests.BijHetRegisteren.VanEenAanvraag.MetOntbrekendeData;

using Aanvragen.Registreer;
using Autofixture;
using Fixture;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenAanvraagZonderVCode : IDisposable
{
    private readonly TestApiClient _client;
    private readonly TestApiFixture _fixture;
    private readonly AanvraagRequest _request;

    public GegevenEenAanvraagZonderVCode(TestApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
        _request = new AutoFixture.Fixture()
            .CustomizeAll()
            .Create<AanvraagRequest>();
        _request.VCode = null!;
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.Aanvragen.RegistreerAanvraag(_request, _client);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        var response = await _client.Aanvragen.RegistreerAanvraag(_request, _client);

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("vCode")
            .WhoseValue
            .Should().ContainEquivalentOf("VCode is verplicht.");
    }

    public void Dispose()
    {
        _fixture.ResetDatabase();
    }
}
