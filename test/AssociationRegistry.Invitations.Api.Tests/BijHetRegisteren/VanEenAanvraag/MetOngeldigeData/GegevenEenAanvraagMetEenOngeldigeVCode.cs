namespace AssociationRegistry.Invitations.Api.Tests.BijHetRegisteren.VanEenAanvraag.MetOngeldigeData;

using Aanvragen.Registreer;
using Autofixture;
using Fixture;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(UitnodigingenApiCollection.Name)]
public class GegevenEenAanvraagMetEenOngeldigeVCode : IDisposable
{
    private readonly UitnodigingenApiClient _client;
    private readonly UitnodigingenApiFixture _fixture;
    private readonly Func<string, AanvraagRequest> _request;

    public GegevenEenAanvraagMetEenOngeldigeVCode(UitnodigingenApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
        _request = s =>
        {
            var aanvraagRequest = new AutoFixture.Fixture()
                .CustomizeAll()
                .Create<AanvraagRequest>();
            aanvraagRequest.VCode = s;
            return aanvraagRequest;
        };
    }

    [Theory]
    [MemberData(nameof(Data))]
    public async Task DanIsDeResponse400(string vCode)
    {
        var response = await _client.RegistreerAanvraag(_request(vCode));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [MemberData(nameof(Data))]
    public async Task DanBevatDeBodyEenErrorMessage(string vCode)
    {
        var response = await _client.RegistreerAanvraag(_request(vCode));

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
