namespace AssociationRegistry.Invitations.Api.Tests.BijHetRegisteren.VanEenAanvraag.MetOngeldigeData;

using Aanvragen.Registreer;
using Autofixture;
using Fixture;
using Newtonsoft.Json.Linq;
using System.Net;

[Collection(TestApiCollection.Name)]
public class GegevenEenAanvragerMetEenOngeldigInsz : IDisposable
{
    private readonly TestApiClient _client;
    private readonly TestApiFixture _fixture;
    private readonly Func<string, AanvraagRequest> _request;

    public GegevenEenAanvragerMetEenOngeldigInsz(TestApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Clients.Authenticated;
        _request = s =>
        {
            var aanvraagRequest = new AutoFixture.Fixture()
                .CustomizeAll()
                .Create<AanvraagRequest>();
            aanvraagRequest.Aanvrager.Insz = s;
            return aanvraagRequest;
        };
    }

    [Theory]
    [MemberData(nameof(Data))]
    public async Task DanIsDeResponse400(string insz)
    {
        var response = await _client.Aanvragen.RegistreerAanvraag(_request(insz));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Theory]
    [MemberData(nameof(Data))]
    public async Task DanBevatDeBodyEenErrorMessage(string insz)
    {
        var response = await _client.Aanvragen.RegistreerAanvraag(_request(insz));

        var content = await response.Content.ReadAsStringAsync();
        var token = JToken.Parse(content);
        token["errors"]!.ToObject<Dictionary<string, string[]>>()
            .Should().ContainKey("aanvrager.Insz")
            .WhoseValue
            .Should().ContainEquivalentOf("Insz is ongeldig. (##.##.##-###.## of ###########)");
    }

    public static IEnumerable<object[]> Data
    {
        get
        {
            yield return new object[] { "random tekst" }; // Geen insz
            yield return new object[] { "0123456789" }; // Te wijnig character
            yield return new object[] { "012345678901234" }; // Te veel nummers
            yield return new object[] { "0.123.45.678.90" }; // . op de verkeerde plaats
            yield return new object[] { "01-02-03.123-08" }; // . en - gewisseld
            yield return new object[] { "01.02.03-123-07" }; // fout modulo
        }
    }

    public void Dispose()
    {
        _fixture.ResetDatabase();
    }
}
