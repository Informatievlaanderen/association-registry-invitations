namespace AssociationRegistry.Invitations.Api.Tests.BijHetRegisteren.VanEenUitnodiging.MetOntbrekendeData;

using Autofixture;
using Fixture;
using Uitnodigingen.Registreer;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenUitnodigerZonderVertegenwoordigersId : IDisposable
{
    private readonly TestApiClient _client;
    private readonly TestApiFixture _fixture;
    private readonly UitnodigingsRequest _request;

    public GegevenEenUitnodigerZonderVertegenwoordigersId(TestApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
        _request = new AutoFixture.Fixture()
            .CustomizeAll()
            .Create<UitnodigingsRequest>();
        _request.Uitnodiger.VertegenwoordigerId = default;
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.Uitnodiging.RegistreerUitnodiging(_request, _client);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        var response = await _client.Uitnodiging.RegistreerUitnodiging(_request, _client);

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("uitnodiger.VertegenwoordigerId")
            .WhoseValue
            .Should().ContainEquivalentOf("VertegenwoordigerId is ongeldig.");
    }

    public void Dispose()
    {
        _fixture.ResetDatabase();
    }
}
