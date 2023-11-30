namespace AssociationRegistry.Invitations.Api.Tests.BijHetRegisteren.VanEenUitnodiging.MetOntbrekendeData;

using Autofixture;
using Fixture;
using Uitnodigingen.Registreer;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenUitnodigingZonderUitnodiger : IDisposable
{
    private readonly TestApiClient _client;
    private readonly TestApiFixture _fixture;
    private readonly UitnodigingsRequest _request;

    public GegevenEenUitnodigingZonderUitnodiger(TestApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
        _request = new AutoFixture.Fixture()
            .CustomizeAll()
            .Create<UitnodigingsRequest>();
        _request.Uitnodiger = null!;
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.Uitnodiging.RegistreerUitnodiging(_request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        var response = await _client.Uitnodiging.RegistreerUitnodiging(_request);

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("uitnodiger")
            .WhoseValue
            .Should().ContainEquivalentOf("Uitnodiger is verplicht.");
    }

    public void Dispose()
    {
        _fixture.ResetDatabase();
    }
}
