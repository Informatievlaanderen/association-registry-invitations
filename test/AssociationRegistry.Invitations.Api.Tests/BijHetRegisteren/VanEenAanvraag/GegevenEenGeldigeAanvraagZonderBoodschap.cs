namespace AssociationRegistry.Invitations.Api.Tests.BijHetRegisteren.VanEenAanvraag;

using Aanvragen.Registreer;
using Autofixture;
using Fixture;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenGeldigeAanvraagZonderBoodschap : IDisposable
{
    private readonly TestApiClient _client;
    private readonly TestApiFixture _fixture;
    private readonly AanvraagRequest _request;

    public GegevenEenGeldigeAanvraagZonderBoodschap(TestApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
        _request = new AutoFixture.Fixture()
            .CustomizeAll()
            .Create<AanvraagRequest>();
        _request.Boodschap = string.Empty;
    }

    [Fact]
    public async Task DanIsDeResponse201()
    {
        var response = await _client.Aanvragen.RegistreerAanvraag(_request, _client);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task DanHeeftDeBodyEenIdDatEenGuidIs()
    {
        var response = await _client.Aanvragen.RegistreerAanvraag(_request, _client);

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        Guid.TryParse(token["aanvraagId"]!.Value<string>(), out _).Should().BeTrue();
    }

    public void Dispose()
    {
        _fixture.ResetDatabase();
    }
}
