using System.Net;
using AssociationRegistry.Invitations.Tests.Autofixture;
using AssociationRegistry.Invitations.Tests.Fixture;
using AssociationRegistry.Invitations.Uitnodingen.Requests;
using Newtonsoft.Json.Linq;

namespace AssociationRegistry.Invitations.Tests.BijHetRegistrerenVanEenUitnodiging;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenUitnodigingMetEenOngeldigeVCode : IDisposable
{
    private readonly UitnodigingenApiClient _client;
    private readonly UitnodigingenApiFixture _fixture;
    private readonly Func<string, UitnodigingsRequest> _request;

    public GegevenEenUitnodigingMetEenOngeldigeVCode(UitnodigingenApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
        _request = s => new UitnodigingenFixture()
            .Build<UitnodigingsRequest>()
            .With(u => u.VCode, s)
            .Create();
    }

    [Theory]
    [MemberData(nameof(Data))]
    public async Task DanIsDeResponse400(string vCode)
    {
        var response = await _client.RegistreerUitnodiging(_request(vCode));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [MemberData(nameof(Data))]
    public async Task DanBevatDeBodyEenErrorMessage(string vCode)
    {
        var response = await _client.RegistreerUitnodiging(_request(vCode));

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("VCode")
            .WhoseValue
            .Should().ContainEquivalentOf("VCode heeft een ongeldig formaat. (V#######)");
    }

    public static IEnumerable<object[]> Data
    {
        get
        {
            yield return new object[] { "random tekst" }; // Geen vcode
            yield return new object[] { "V012345" }; // Te wijnig character
            yield return new object[] { "V01234567" }; // Te veel nummers
            yield return new object[] { "W0123456" }; // Geen V als eerste char

        }
    }

    public void Dispose()
    {
        _fixture.RestDatabase();
    }
}
