namespace AssociationRegistry.Invitations.Api.Tests.BijHetRegisteren.VanEenAanvraag.MetOntbrekendeData;

using Aanvragen.Registreer;
using Autofixture;
using Fixture;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenAanvragerZonderInsz : IDisposable
{
    private readonly TestApiClient _client;
    private readonly TestApiFixture _fixture;
    private readonly AanvraagRequest _request;

    public GegevenEenAanvragerZonderInsz(TestApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
        _request = new AutoFixture.Fixture()
            .CustomizeAll()
            .Create<AanvraagRequest>();
        _request.Aanvrager.Insz = null!;
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.Aanvragen.RegistreerAanvraag(_request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        var response = await _client.Aanvragen.RegistreerAanvraag(_request);

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("aanvrager.Insz")
            .WhoseValue
            .Should().ContainEquivalentOf("Insz is verplicht.");
    }

    public void Dispose()
    {
        _fixture.ResetDatabase();
    }
}
