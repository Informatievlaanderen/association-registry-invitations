namespace AssociationRegistry.Invitations.Api.Tests.BijHetWeigeren.VanEenUitnodiging;

using AssociationRegistry.Invitations.Api.Tests.Fixture;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenOnbekendeUitnodiging : IDisposable
{
    private readonly TestApiFixture _fixture;
    private readonly TestApiClient _client;

    public GegevenEenOnbekendeUitnodiging(TestApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
    }

    [Fact]
    public async Task DanIsDeResponse400()
    {
        var response = await _client.Uitnodiging.WeigerUitnodiging(Guid.NewGuid());
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DanBevatDeBodyEenErrorMessage()
    {
        var response = await _client.Uitnodiging.WeigerUitnodiging(Guid.NewGuid());

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("uitnodiging")
            .WhoseValue
            .Should().ContainEquivalentOf("Deze uitnodiging is niet gekend.");
    }


    public void Dispose()
    {
        _fixture.ResetDatabase();
    }
}
