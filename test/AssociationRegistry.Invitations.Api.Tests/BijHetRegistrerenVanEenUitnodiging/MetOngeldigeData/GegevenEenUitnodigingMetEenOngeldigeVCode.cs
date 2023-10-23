using System.Net;
using AssociationRegistry.Invitations.Api.Tests.Autofixture;
using AssociationRegistry.Invitations.Api.Tests.Fixture;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Registreer;
using Newtonsoft.Json.Linq;

namespace AssociationRegistry.Invitations.Api.Tests.BijHetRegistrerenVanEenUitnodiging.MetOngeldigeData;

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
        _request = s =>
        {
            var uitnodigingsRequest = new AutoFixture.Fixture()
                .CustomizeAll()
                .Create<UitnodigingsRequest>();
            uitnodigingsRequest.VCode = s;
            return uitnodigingsRequest;
        };
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
            .Should().ContainKey("vCode")
            .WhoseValue
            .Should().ContainEquivalentOf("VCode heeft een ongeldig formaat. (V#######)");
    }

    public static IEnumerable<object[]> Data
    {
        get
        {
            yield return new object[] { "random tekst" }; // Geen vcode
            yield return new object[] { "V012345" }; // Te weinig character
            yield return new object[] { "V01234567" }; // Te veel nummers
            yield return new object[] { "W0123456" }; // Geen V als eerste char
        }
    }

    public void Dispose()
    {
        _fixture.ResetDatabase();
    }
}
